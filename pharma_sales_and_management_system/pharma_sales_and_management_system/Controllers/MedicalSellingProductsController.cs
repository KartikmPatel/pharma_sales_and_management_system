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
    public class MedicalSellingProductsController : Controller
    {
        private readonly pharma_managementContext _context;

        public MedicalSellingProductsController(pharma_managementContext context)
        {
            _context = context;
        }

        private bool IsUserAuthenticated()
        {
            return HttpContext.Session.GetInt32("MedicalShopId").HasValue;
        }

        // GET: MedicalSellingProducts
        public async Task<IActionResult> Index()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "MedicalShopRegister");
            }
            else
            {
                var medicalShopId = HttpContext.Session.GetInt32("MedicalShopId");

                var pharma_managementContext = _context.MedicalSellingProducts.Where(m => m.MedicalShopId == medicalShopId).Include(m => m.MedicalShop).Include(m => m.Product);
                return View(await pharma_managementContext.ToListAsync());
            }
        }

        // GET: MedicalSellingProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MedicalSellingProducts == null)
            {
                return NotFound();
            }

            var medicalSellingProduct = await _context.MedicalSellingProducts
                .Include(m => m.MedicalShop)
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalSellingProduct == null)
            {
                return NotFound();
            }

            return View(medicalSellingProduct);
        }

        // GET: MedicalSellingProducts/Create
        public IActionResult Create()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "MedicalShopRegister");
            }
            else
            {
                var medicalShopId = HttpContext.Session.GetInt32("MedicalShopId");

                var prodIDs = (from p in _context.MedicalShopProductStocks
                               where p.MedicalShopId == medicalShopId
                               select p.ProductId).ToList();

                var productNames = from n in _context.ProductDetails.ToList()
                                   where prodIDs.Contains(n.Id)
                                   select n.ProductName;

                // Convert productNames to a list of SelectListItem
                var selectListItems = productNames.Select(name => new SelectListItem { Text = name, Value = name }).ToList();

                // Assign the list of SelectListItem to ViewBag.ProductName
                ViewBag.ProductName = selectListItems;

                return View();
            }
        }

        [HttpPost]
        public int getPrice(string productName)
        {
            var product_Price = (from p in _context.ProductDetails
                                 where p.ProductName == productName
                                 select p.RetailPrice).FirstOrDefault();
            return product_Price;
        }

        // POST: MedicalSellingProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Mrp,MedicalShopId")] MedicalSellingProduct medicalSellingProduct,string pname)
        {
            var medicalShopId = HttpContext.Session.GetInt32("MedicalShopId");
            var productId = await (from i in _context.ProductDetails
                                       where i.ProductName == pname
                                       select i.Id).FirstOrDefaultAsync();

                medicalSellingProduct.ProductId = productId;
                medicalSellingProduct.MedicalShopId = Convert.ToInt32(medicalShopId);

                var sellingDetails = await (from c in _context.MedicalSellingProducts
                                 where c.ProductId == productId && c.MedicalShopId == medicalShopId
                                 select c).FirstOrDefaultAsync();

                if(sellingDetails != null)
                {
                    var prodIDs = (from p in _context.MedicalShopProductStocks
                               where p.MedicalShopId == medicalShopId
                               select p.ProductId).ToList();

                    var productNames = from n in _context.ProductDetails.ToList()
                                   where prodIDs.Contains(n.Id)
                                   select n.ProductName;

                    var selectListItems = productNames.Select(name => new SelectListItem { Text = name, Value = name }).ToList();

                    ViewBag.ProductName = selectListItems;
                    TempData["repeatDataError"] = "Medicine is already in selling ";
                    return View(medicalSellingProduct);
                }
                else
                {
                    _context.Add(medicalSellingProduct);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            return Problem("Entity set 'pharma_managementContext.MedicalSellingProduct'  is null.");
        }

        // GET: MedicalSellingProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MedicalSellingProducts == null)
            {
                return NotFound();
            }

            var medicalSellingProduct = await _context.MedicalSellingProducts.FindAsync(id);
            if (medicalSellingProduct == null)
            {
                return NotFound();
            }
            ViewData["MedicalShopId"] = new SelectList(_context.MedicalShopDetails, "Id", "Id", medicalSellingProduct.MedicalShopId);
            ViewData["ProductId"] = new SelectList(_context.ProductDetails, "Id", "Id", medicalSellingProduct.ProductId);
            return View(medicalSellingProduct);
        }

        // POST: MedicalSellingProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Mrp,MedicalShopId")] MedicalSellingProduct medicalSellingProduct)
        {
            if (id != medicalSellingProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalSellingProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalSellingProductExists(medicalSellingProduct.Id))
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
            ViewData["MedicalShopId"] = new SelectList(_context.MedicalShopDetails, "Id", "Id", medicalSellingProduct.MedicalShopId);
            ViewData["ProductId"] = new SelectList(_context.ProductDetails, "Id", "Id", medicalSellingProduct.ProductId);
            return View(medicalSellingProduct);
        }

        // GET: MedicalSellingProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MedicalSellingProducts == null)
            {
                return NotFound();
            }

            var medicalSellingProduct = await _context.MedicalSellingProducts
                .Include(m => m.MedicalShop)
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalSellingProduct == null)
            {
                return NotFound();
            }

            return View(medicalSellingProduct);
        }

        // POST: MedicalSellingProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MedicalSellingProducts == null)
            {
                return Problem("Entity set 'pharma_managementContext.MedicalSellingProducts'  is null.");
            }
            var medicalSellingProduct = await _context.MedicalSellingProducts.FindAsync(id);
            if (medicalSellingProduct != null)
            {
                _context.MedicalSellingProducts.Remove(medicalSellingProduct);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalSellingProductExists(int id)
        {
          return (_context.MedicalSellingProducts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
