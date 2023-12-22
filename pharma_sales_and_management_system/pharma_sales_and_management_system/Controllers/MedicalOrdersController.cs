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
    public class MedicalOrdersController : Controller
    {
        private readonly pharma_managementContext _context;

        public MedicalOrdersController(pharma_managementContext context)
        {
            _context = context;
        }

        private bool IsUserAuthenticated()
        {
            return HttpContext.Session.GetInt32("MedicalShopId").HasValue;
        }

        // GET: MedicalOrders
        public async Task<IActionResult> Index()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "MedicalShopRegister");
            }
            else
            {
                var medicalShopId = HttpContext.Session.GetInt32("MedicalShopId");
                var medicalDetails = await (from i in _context.MedicalShopDetails
                                            where i.Id == medicalShopId
                                            select i).FirstOrDefaultAsync();
                ViewBag.ProfilePhoto = medicalDetails.ProfilePic;
                ViewBag.editId = medicalDetails.Id;
                var pharma_managementContext = await (from o in _context.MedicalOrders
                                                      where o.MedicalShopId == medicalShopId
                                                      select o).Include(m => m.Company).Include(m => m.MedicalShop).Include(m => m.Product).ToListAsync();

                ViewBag.Success = TempData["success"];
                return View(pharma_managementContext);
            }
        }

        // GET: MedicalOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "MedicalShopRegister");
            }
            if (id == null || _context.MedicalOrders == null)
            {
                return NotFound();
            }

            var medicalOrder = await _context.MedicalOrders
                .Include(m => m.Company)
                .Include(m => m.MedicalShop)
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalOrder == null)
            {
                return NotFound();
            }

            return View(medicalOrder);
        }

        // GET: MedicalOrders/Create
        public IActionResult Create()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "MedicalShopRegister");
            }
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id");
            ViewData["MedicalShopId"] = new SelectList(_context.MedicalShopDetails, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.ProductDetails, "Id", "Id");
            return View();
        }

        // POST: MedicalOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MedicalShopId,ProductId,CompanyId,TotalQuantity,BillAmount,OrderDate,IsPlaced")] MedicalOrder medicalOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicalOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id", medicalOrder.CompanyId);
            ViewData["MedicalShopId"] = new SelectList(_context.MedicalShopDetails, "Id", "Id", medicalOrder.MedicalShopId);
            ViewData["ProductId"] = new SelectList(_context.ProductDetails, "Id", "Id", medicalOrder.ProductId);
            return View(medicalOrder);
        }

        // GET: MedicalOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "MedicalShopRegister");
            }
            if (id == null || _context.MedicalOrders == null)
            {
                return NotFound();
            }

            var medicalOrder = await _context.MedicalOrders.FindAsync(id);
            if (medicalOrder == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id", medicalOrder.CompanyId);
            ViewData["MedicalShopId"] = new SelectList(_context.MedicalShopDetails, "Id", "Id", medicalOrder.MedicalShopId);
            ViewData["ProductId"] = new SelectList(_context.ProductDetails, "Id", "Id", medicalOrder.ProductId);
            return View(medicalOrder);
        }

        // POST: MedicalOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MedicalShopId,ProductId,CompanyId,TotalQuantity,BillAmount,OrderDate,IsPlaced")] MedicalOrder medicalOrder)
        {
            if (id != medicalOrder.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalOrderExists(medicalOrder.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id", medicalOrder.CompanyId);
            ViewData["MedicalShopId"] = new SelectList(_context.MedicalShopDetails, "Id", "Id", medicalOrder.MedicalShopId);
            ViewData["ProductId"] = new SelectList(_context.ProductDetails, "Id", "Id", medicalOrder.ProductId);
            return View(medicalOrder);
        }

        // GET: MedicalOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "MedicalShopRegister");
            }
            if (id == null || _context.MedicalOrders == null)
            {
                return NotFound();
            }

            var medicalShopId = HttpContext.Session.GetInt32("MedicalShopId");
            var medicalDetails = (from i in _context.MedicalShopDetails
                                  where i.Id == medicalShopId
                                  select i).FirstOrDefault();
            ViewBag.ProfilePhoto = medicalDetails.ProfilePic;
            ViewBag.editId = medicalDetails.Id;
            var medicalOrder = await _context.MedicalOrders
                .Include(m => m.Company)
                .Include(m => m.MedicalShop)
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalOrder == null)
            {
                return NotFound();
            }

            return View(medicalOrder);
        }

        // POST: MedicalOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MedicalOrders == null)
            {
                return Problem("Entity set 'pharma_managementContext.MedicalOrders'  is null.");
            }
            var medicalOrder = await _context.MedicalOrders.FindAsync(id);
            if (medicalOrder != null)
            {
                _context.MedicalOrders.Remove(medicalOrder);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Medicine Successfully Deleted";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult UserOrder()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "MedicalShopRegister");
            }
            else
            {
                var medicalShopId = HttpContext.Session.GetInt32("MedicalShopId");
                var medicalDetails = (from i in _context.MedicalShopDetails
                                           where i.Id == medicalShopId
                                           select i).FirstOrDefault();
                ViewBag.ProfilePhoto = medicalDetails.ProfilePic;
                var data = from o in _context.UserOrders
                           where o.MedicalShopId == medicalShopId
                           select new UserOrderView
                           {
                               Id = o.Id,
                               productName = (from p in _context.ProductDetails
                                              where p.Id == o.ProductId
                                              select p.ProductName).FirstOrDefault(),
                               Image = (from p in _context.ProductDetails
                                        where p.Id == o.ProductId
                                        select p.ProductImage).FirstOrDefault(),
                               userName = (from u in _context.UserDetails
                                           where u.Id == o.UserId
                                           select u.Name).FirstOrDefault(),
                               quantity = o.Quantity,
                               totalAmount = o.TotalAmount,
                               orderDate = o.OrderDate,
                               isDelivered = o.IsDelivered,
                               orderAddress = o.OrderAddress
                           };
                return View(data);
            }
        }

        [HttpPost]
        public IActionResult AcceptUserOrder(int oId)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "MedicalShopRegister");
            }
            else
            {
                var orderData = _context.UserOrders.FirstOrDefault(x=>x.Id == oId);

                var stockData = _context.MedicalShopProductStocks.FirstOrDefault(x=>x.ProductId == orderData.ProductId && x.MedicalShopId == orderData.MedicalShopId);

                if(stockData.TotalQuantity < orderData.Quantity)
                {
                    TempData["lessQuantity"] = "Not Sufficient Quantity for Product";
                    return RedirectToAction(nameof(UserOrder));
                }
                else
                {
                    stockData.TotalQuantity = stockData.TotalQuantity - orderData.Quantity;

                    orderData.IsDelivered = 1;
                    _context.SaveChanges();

                    TempData["OrderDone"] = "Order Successfully Confirmed";
                    return RedirectToAction(nameof(UserOrder));
                }
                return View();
            }
        }

        private bool MedicalOrderExists(int id)
        {
            return (_context.MedicalOrders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}