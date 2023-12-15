using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using pharma_sales_and_management_system.Models;

namespace pharma_sales_and_management_system.Controllers
{
    public class MedicalMedicineController : Controller
    {
        private readonly pharma_managementContext _context;
        private readonly IWebHostEnvironment _webHostEnv;

        public MedicalMedicineController(pharma_managementContext context, IWebHostEnvironment webHostEnv)
        {
            _context = context;
            _webHostEnv = webHostEnv;
        }

        private bool IsUserAuthenticated()
        {
            return HttpContext.Session.GetInt32("MedicalShopId").HasValue;
        }

        // GET: MedicalMedicine
        public async Task<IActionResult> Index()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "MedicalShopRegister");
            }
            else
            {
                var medicalShopId = HttpContext.Session.GetInt32("MedicalShopId");

                var Confirmed = await (from c in _context.MedicalShopDetails
                                       where c.IsConfirmed == 1 && c.Id == medicalShopId
                                       select c.IsConfirmed).FirstOrDefaultAsync();
                var pharma_managementContext = _context.ProductDetails.Include(p => p.Category).Include(p => p.Company);
                ViewBag.Confirmed = Confirmed;
                return View(await pharma_managementContext.ToListAsync());
            }
        }

        // GET: MedicalMedicine/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductDetails == null)
            {
                return NotFound();
            }

            var productDetail = await _context.ProductDetails
                .Include(p => p.Category)
                .Include(p => p.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productDetail == null)
            {
                return NotFound();
            }

            return View(productDetail);
        }

        // GET: MedicalMedicine/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Id");
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id");
            return View();
        }

        // POST: MedicalMedicine/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductName,RetailPrice,ProductImage,Description,Disease,CategoryId,MfgDate,CompanyId,ExpDate")] ProductDetail productDetail, IFormFile file)
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
                productDetail.ProductImage = @"\images\user\" + filename;
            }

            _context.Add(productDetail);
            await _context.SaveChangesAsync();

            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Id", productDetail.CategoryId);
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id", productDetail.CompanyId);
            return RedirectToAction(nameof(Index));
        }

        // GET: MedicalMedicine/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProductDetails == null)
            {
                return NotFound();
            }

            var productDetail = await _context.ProductDetails.FindAsync(id);
            if (productDetail == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Id", productDetail.CategoryId);
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id", productDetail.CompanyId);
            return View(productDetail);
        }

        // POST: MedicalMedicine/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,RetailPrice,ProductImage,Description,Disease,CategoryId,MfgDate,CompanyId,ExpDate")] ProductDetail productDetail, IFormFile file,string oldfile)
        {
            if (id != productDetail.Id)
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
                    productDetail.ProductImage = @"\images\user\" + filename;
                }
                    else if (!string.IsNullOrEmpty(oldfile))
                    {
                        productDetail.ProductImage = oldfile;
                    }
                    _context.Update(productDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductDetailExists(productDetail.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                    
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Id", productDetail.CategoryId);
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id", productDetail.CompanyId);
            return RedirectToAction(nameof(Index));
        }

        // GET: MedicalMedicine/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductDetails == null)
            {
                return NotFound();
            }

            var productDetail = await _context.ProductDetails
                .Include(p => p.Category)
                .Include(p => p.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productDetail == null)
            {
                return NotFound();
            }

            return View(productDetail);
        }

        // POST: MedicalMedicine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductDetails == null)
            {
                return Problem("Entity set 'pharma_managementContext.ProductDetails'  is null.");
            }
            var productDetail = await _context.ProductDetails.FindAsync(id);
            if (productDetail != null)
            {
                _context.ProductDetails.Remove(productDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductDetailExists(int id)
        {
          return (_context.ProductDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost]
        public ActionResult PurchaseMedicine(string productName,int companyId)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "MedicalShopRegister");
            }
            else
            {
                var medicalShopId = HttpContext.Session.GetInt32("MedicalShopId");

                var companyName = (from c in _context.Manufacturers
                                  where c.Id == companyId
                                  select c.ComponyName).FirstOrDefault();
                ViewBag.medicalShopId = medicalShopId;
                ViewBag.productName = productName;
                ViewBag.companyName = companyName;
            return View();
            }
        }

        [HttpPost]
        public int billCalculation(int totalQuantity,string productName)
        {
            var product_Price = (from p in _context.ProductDetails
                                 where p.ProductName == productName
                                 select p.RetailPrice).FirstOrDefault();

            var totalAmount = product_Price * totalQuantity;

            return totalAmount;
        }

        [HttpPost]
        public ActionResult purchaseProduct(int medicalShopId,string productName,string companyName,int TotalQuantity,int BillAmount,MedicalOrder medicalOrder)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "MedicalShopRegister");
            }
            else
            {
            var productId = (from p in _context.ProductDetails
                            where p.ProductName == productName
                            select p.Id).FirstOrDefault();
            var companyId = (from c in _context.Manufacturers
                             where c.ComponyName == companyName
                             select c.Id).FirstOrDefault();

            medicalOrder.MedicalShopId = medicalShopId;
            medicalOrder.ProductId = productId;
            medicalOrder.CompanyId = companyId;
            medicalOrder.TotalQuantity = TotalQuantity;
            medicalOrder.BillAmount = BillAmount;
            medicalOrder.OrderDate = DateTime.Now;
            medicalOrder.IsPlaced = 0;

            _context.MedicalOrders.Add(medicalOrder);
            _context.SaveChanges();

             TempData["success"] = "Orders Successfully Placed";
            return RedirectToAction(nameof(Index));
            }
        }
    }
}
