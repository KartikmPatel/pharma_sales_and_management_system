using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pharma_sales_and_management_system.Models;

namespace pharma_sales_and_management_system.Controllers
{
    public class UserController : Controller
    {
        private readonly pharma_managementContext _context;
        private readonly IWebHostEnvironment _webHostEnv;

        public UserController(pharma_managementContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnv = webHostEnvironment;
        }

        private bool IsUserAuthenticated()
        {
            return HttpContext.Session.GetInt32("UserId").HasValue;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Id,Email,Password")] UserDetail userDetail)
        {
            var user = _context.UserDetails.FirstOrDefault(u => u.Email == userDetail.Email && u.Password == userDetail.Password);

            if (user != null)
            {
                // Store user's Id in session
                HttpContext.Session.SetInt32("UserId", user.Id);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["InvalidLogin"] = "Invalid Username OR Password";
                return RedirectToAction(nameof(Login));
            }
        }

        public IActionResult Logout()
        {
            // Clear user's session data
            HttpContext.Session.Remove("UserId");

            // Redirect to the login page or any other page after logout
            return RedirectToAction(nameof(Login));
        }

        // GET: User
        public async Task<IActionResult> Index(string product,int? categoryId)
        {
            var UserId = HttpContext.Session.GetInt32("UserId");
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                if (categoryId != null)
                {
                    var productIds = (from p in _context.ProductDetails
                                      where p.CategoryId == categoryId
                                      select p.Id).ToList();

                    var categoryData = from m in _context.MedicalSellingProducts
                               where productIds.Contains(m.ProductId)
                               select new MedicalSellingProductViewModel
                               {
                                   Id = m.Id,
                                   Mrp = m.Mrp,
                                   ProductName = (from p in _context.ProductDetails
                                                  where p.Id == m.ProductId
                                                  select p.ProductName).FirstOrDefault(),
                                   Image = (from p in _context.ProductDetails
                                            where p.Id == m.ProductId
                                            select p.ProductImage).FirstOrDefault()
                               };

                    ViewBag.category = new SelectList(_context.ProductCategories.ToList(), "Id", "CategoryName");
                    return Json(categoryData);
                }
                else
                {
                    if (product != null)
                    {
                        var productId = (from p in _context.ProductDetails
                                         where p.ProductName.Contains(product)
                                         select p.Id).FirstOrDefault();

                        var searchData = from m in _context.MedicalSellingProducts
                                         where m.ProductId == productId
                                         select new MedicalSellingProductViewModel
                                         {
                                             Id = m.Id,
                                             Mrp = m.Mrp,
                                             ProductName = (from p in _context.ProductDetails
                                                            where p.Id == m.ProductId
                                                            select p.ProductName).FirstOrDefault(),
                                             Image = (from p in _context.ProductDetails
                                                      where p.Id == m.ProductId
                                                      select p.ProductImage).FirstOrDefault()
                                         };

                        ViewBag.category = new SelectList(_context.ProductCategories.ToList(), "Id", "CategoryName");
                        return Json(searchData);
                    }
                    else
                    {
                        var data = from m in _context.MedicalSellingProducts
                                   select new MedicalSellingProductViewModel
                                   {
                                       Id = m.Id,
                                       Mrp = m.Mrp,
                                       ProductName = (from p in _context.ProductDetails
                                                      where p.Id == m.ProductId
                                                      select p.ProductName).FirstOrDefault(),
                                       Image = (from p in _context.ProductDetails
                                                where p.Id == m.ProductId
                                                select p.ProductImage).FirstOrDefault()
                                   };
                        ViewBag.category = new SelectList(_context.ProductCategories.ToList(), "Id", "CategoryName");
                        var uDetails = await (from u in _context.UserDetails
                                              where u.Id == UserId
                                              select u).FirstOrDefaultAsync();
                        ViewBag.ProfilePhoto = uDetails.ProfilePic;
                        ViewBag.editId = uDetails.Id;
                        return View(data);
                    }

                }
            }
        }

        public IActionResult DetailProduct(int mid,int mrp,string productName)
        {
            var UserId = HttpContext.Session.GetInt32("UserId");
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                var medicalSellingData = _context.MedicalSellingProducts.FirstOrDefault(x => x.Id == mid);
                var medicalData = _context.MedicalShopDetails.FirstOrDefault(x => x.Id == medicalSellingData.MedicalShopId);
                var productData = _context.ProductDetails.FirstOrDefault(x => x.Id == medicalSellingData.ProductId);
                var companyData = _context.Manufacturers.FirstOrDefault(x => x.Id == productData.CompanyId);
                var categoryData = _context.ProductCategories.FirstOrDefault(x => x.Id == productData.CategoryId);

                var uDetails = (from u in _context.UserDetails
                                     where u.Id == UserId
                                     select u).FirstOrDefault();
                ViewBag.ProfilePhoto = uDetails.ProfilePic;
                ViewBag.editId = uDetails.Id;
                return View((productData.Id,productData.ProductName,productData.Description,medicalSellingData.Mrp,productData.Disease,companyData.ComponyName,productData.ProductImage,productData.ExpDate,categoryData.CategoryName,medicalData.Id));
            }
        }

        public IActionResult addCart(int mid, int pid, UserCart userCart)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                var userId = HttpContext.Session.GetInt32("UserId");

                userCart.UserId = Convert.ToInt32(userId);
                userCart.ProductId = pid;
                userCart.Quantity = 1;
                userCart.MedicalShopId = mid;

                var existingCartItem = _context.UserCarts
                    .FirstOrDefault(c => c.MedicalShopId == userCart.MedicalShopId && c.ProductId == userCart.ProductId && c.UserId == userId);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += 1;
                    _context.UserCarts.Update(existingCartItem);
                }
                else
                {
                    _context.UserCarts.Add(userCart);
                }

                _context.SaveChanges();
                var uDetails = (from u in _context.UserDetails
                                where u.Id == userId
                                select u).FirstOrDefault();
                ViewBag.ProfilePhoto = uDetails.ProfilePic;
                ViewBag.editId = uDetails.Id;
                TempData["CartAdded"] = "Product Successfully Add in Cart";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Cart()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                var userId = HttpContext.Session.GetInt32("UserId");

                var data = _context.UserCarts
                    .Where(c => c.UserId == userId)
                    .Select(c => new CartViewModel
                    {
                        Id = c.Id,
                        ProductName = _context.ProductDetails
                            .Where(p => p.Id == c.ProductId)
                            .Select(p => p.ProductName)
                            .FirstOrDefault(),
                        Mrp = _context.MedicalSellingProducts
                            .Where(m => m.ProductId == c.ProductId && m.MedicalShopId == c.MedicalShopId)
                            .Select(m => m.Mrp)
                            .FirstOrDefault(),
                        ProductImage = _context.ProductDetails
                            .Where(i => i.Id == c.ProductId)
                            .Select(i => i.ProductImage)
                            .FirstOrDefault(),
                        Quantity = c.Quantity,
                        productId = c.ProductId,
                        medicalId = c.MedicalShopId
                    })
                    .ToList();

                var uDetails = (from u in _context.UserDetails
                                where u.Id == userId
                                select u).FirstOrDefault();
                ViewBag.ProfilePhoto = uDetails.ProfilePic;
                ViewBag.editId = uDetails.Id;
                return View(data);
            }
        }

        [HttpPost]
        public IActionResult UpdateCartItemQuantity(int itemId, string operation)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                var cartItem = _context.UserCarts.FirstOrDefault(x => x.Id == itemId);

                if(cartItem == null)
                {
                    return NotFound();
                }

                if(operation == "increment")
                {
                    cartItem.Quantity++;
                }
                else
                {
                    if(operation == "decrement" && cartItem.Quantity > 1)
                    {
                        cartItem.Quantity--;
                    }
                }

                _context.SaveChanges();
                return Json(cartItem.Quantity);
            }
        }

        public IActionResult RemoveCartItem(int cid)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                var cartItem = _context.UserCarts.FirstOrDefault(i => i.Id == cid);

                _context.UserCarts.Remove(cartItem);
                _context.SaveChanges();

                var userId = HttpContext.Session.GetInt32("UserId");
                var uDetails = (from u in _context.UserDetails
                                where u.Id == userId
                                select u).FirstOrDefault();
                ViewBag.ProfilePhoto = uDetails.ProfilePic;
                ViewBag.editId = uDetails.Id;
                TempData["CartDeleted"] = "Product Successfully Removed";
                return RedirectToAction(nameof(Cart));
            }
        }

        [HttpPost]
        public IActionResult Order(List<int> cIds, List<int> mrps)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                using (var dbContext = _context)
                {
                    var userId = HttpContext.Session.GetInt32("UserId");
                    var user = _context.UserDetails.FirstOrDefault(x => x.Id == userId);

                    for (int i = 0; i < cIds.Count; i++)
                    {
                        var cart = _context.UserCarts.FirstOrDefault(x=>x.Id == cIds[i]);
                        var order = new UserOrder
                        {
                            ProductId = cart.ProductId,
                            MedicalShopId = cart.MedicalShopId,
                            UserId = Convert.ToInt32(userId),
                            Quantity = cart.Quantity,
                            TotalAmount = cart.Quantity * mrps[i], 
                            OrderDate = DateTime.Now,
                            IsDelivered = 0,
                            OrderAddress = user.City,
                            OrderCity = user.City
                };
                        dbContext.UserOrders.Add(order);
                        dbContext.UserCarts.Remove(cart);
                    
                    }
                    dbContext.SaveChanges();
                }
                TempData["OrderSuccess"] = "Order Successfully Placed";
                return RedirectToAction(nameof(Index));

            }
        }

        public IActionResult OrderHistory()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
              
                var userId = HttpContext.Session.GetInt32("UserId");
                var data = _context.UserOrders
                    .Where(o => o.UserId == userId)
                    .Select(o => new OrderViewModel
                    {
                        Id = o.Id,
                        totalAmount = o.TotalAmount,
                        Quantity = o.Quantity,
                        Image = _context.ProductDetails
                            .Where(i => i.Id == o.ProductId)
                            .Select(i => i.ProductImage)
                            .FirstOrDefault(),
                        productName = _context.ProductDetails
                            .Where(i => i.Id == o.ProductId)
                            .Select(i => i.ProductName)
                            .FirstOrDefault(),
                        is_delivered = o.IsDelivered
                    })
                    .ToList();

                var uDetails = (from u in _context.UserDetails
                                where u.Id == userId
                                select u).FirstOrDefault();
                ViewBag.ProfilePhoto = uDetails.ProfilePic;
                ViewBag.editId = uDetails.Id;
                return View(data);
            }
        }

        public IActionResult OrderDetail(int oId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                var orderData = _context.UserOrders.FirstOrDefault(x => x.Id == oId);
                var productData = _context.ProductDetails.FirstOrDefault(x => x.Id == orderData.ProductId);
                var medicalSellingData = _context.MedicalSellingProducts.FirstOrDefault(x => x.MedicalShopId == orderData.MedicalShopId && x.ProductId == orderData.ProductId);
                var categoryData = _context.ProductCategories.FirstOrDefault(x => x.Id == productData.CategoryId);
                var uDetails = (from u in _context.UserDetails
                                where u.Id == userId
                                select u).FirstOrDefault();
                ViewBag.ProfilePhoto = uDetails.ProfilePic;
                ViewBag.editId = uDetails.Id;
                return View((orderData.Id,orderData.Quantity,orderData.TotalAmount,orderData.IsDelivered,orderData.OrderAddress,productData.Id,productData.ProductName,categoryData.CategoryName,productData.ProductImage,medicalSellingData.Mrp));
            }
        }
        public async Task<IActionResult> Details(int? id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }

            if (id == null || _context.UserDetails == null)
            {
                return NotFound();
            }

            var userDetail = await _context.UserDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userDetail == null)
            {
                return NotFound();
            }

            var uDetails = (from u in _context.UserDetails
                            where u.Id == userId
                            select u).FirstOrDefault();
            ViewBag.ProfilePhoto = uDetails.ProfilePic;
            ViewBag.editId = uDetails.Id;
            return View(userDetail);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password,City,ContactNo,Pincode,ProfilePic")] UserDetail userDetail, IFormFile file)
        {
            string wwwRootPath = this._webHostEnv.WebRootPath;
            if (file != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productpath = Path.Combine(wwwRootPath, @"images\user");
                using (var filestream = new FileStream(Path.Combine(productpath, filename), FileMode.Create))
                {
                    file.CopyTo(filestream);
                }
                userDetail.ProfilePic = @"\images\user\" + filename;
            }

            _context.Add(userDetail);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("UserId", userDetail.Id);
            return RedirectToAction(nameof(Index));
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }

            if (id == null || _context.UserDetails == null)
            {
                return NotFound();
            }

            var userDetail = await _context.UserDetails.FindAsync(id);
            if (userDetail == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            var uDetails = (from u in _context.UserDetails
                            where u.Id == userId
                            select u).FirstOrDefault();
            ViewBag.ProfilePhoto = uDetails.ProfilePic;
            ViewBag.editId = uDetails.Id;
            ViewBag.successmessage = TempData["success"];
            return View(userDetail);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password,City,ContactNo,Pincode,ProfilePic")] UserDetail userDetail, IFormFile file,string oldfile)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }

            if (id != userDetail.Id)
            {
                return NotFound();
            }

            try
            {
                string wwwRootPath = this._webHostEnv.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productpath = Path.Combine(wwwRootPath, @"images\user");
                    using (var filestream = new FileStream(Path.Combine(productpath, filename), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    userDetail.ProfilePic = @"\images\user\" + filename;
                }
                else if (!string.IsNullOrEmpty(oldfile))
                {
                    userDetail.ProfilePic = oldfile;
                }

                _context.Update(userDetail);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDetailExists(userDetail.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            TempData["success"] = "Profile Successfully Edited";
            return RedirectToAction(nameof(Edit));
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }

            if (id == null || _context.UserDetails == null)
            {
                return NotFound();
            }

            var userDetail = await _context.UserDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userDetail == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            var uDetails = (from u in _context.UserDetails
                            where u.Id == userId
                            select u).FirstOrDefault();
            ViewBag.ProfilePhoto = uDetails.ProfilePic;
            ViewBag.editId = uDetails.Id;
            return View(userDetail);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }

            if (_context.UserDetails == null)
            {
                return Problem("Entity set 'pharma_managementContext.UserDetails'  is null.");
            }
            var userDetail = await _context.UserDetails.FindAsync(id);
            if (userDetail != null)
            {
                _context.UserDetails.Remove(userDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserDetailExists(int id)
        {
          return (_context.UserDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
