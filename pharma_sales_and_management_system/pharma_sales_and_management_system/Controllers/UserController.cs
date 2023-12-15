using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> Index()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
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

                return View(data);
            }
        }

        public IActionResult DetailProduct(int mid,int mrp,string productName)
        {
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

            return View((productData.Id,productData.ProductName,productData.Description,productData.RetailPrice,productData.Disease,companyData.ComponyName,productData.ProductImage,productData.ExpDate,categoryData.CategoryName,medicalData.Id));
            }
        }

        public IActionResult addCart(int mid,int pid,UserCart userCart)
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
                _context.UserCarts.Add(userCart);
                 _context.SaveChanges();

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

                TempData["CartDeleted"] = "Product Successfully Removed";
                return RedirectToAction(nameof(Cart));
            }
        }
        public async Task<IActionResult> Details(int? id)
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
            return RedirectToAction(nameof(Index));
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
