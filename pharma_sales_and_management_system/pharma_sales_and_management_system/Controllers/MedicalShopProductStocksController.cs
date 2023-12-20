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
    public class MedicalShopProductStocksController : Controller
    {
        private readonly pharma_managementContext _context;

        public MedicalShopProductStocksController(pharma_managementContext context)
        {
            _context = context;
        }

        private bool IsUserAuthenticated()
        {
            return HttpContext.Session.GetInt32("MedicalShopId").HasValue;
        }

        // GET: MedicalShopProductStocks
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
                var pharma_managementContext = _context.MedicalShopProductStocks.Where(m => m.MedicalShopId == medicalShopId).Include(m => m.MedicalShop).Include(m => m.Product);
                return View(await pharma_managementContext.ToListAsync());
            }
        }

        // GET: MedicalShopProductStocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "MedicalShopRegister");
            }
            if (id == null || _context.MedicalShopProductStocks == null)
            {
                return NotFound();
            }

            var medicalShopProductStock = await _context.MedicalShopProductStocks
                .Include(m => m.MedicalShop)
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalShopProductStock == null)
            {
                return NotFound();
            }

            return View(medicalShopProductStock);
        }

        // GET: MedicalShopProductStocks/Create
        public IActionResult Create()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "MedicalShopRegister");
            }
            ViewBag.MedicalShopId = new SelectList(_context.MedicalShopDetails, "Id", "OwnerName");
            ViewBag.ProductId = new SelectList(_context.ProductDetails, "Id", "ProductName");
            return View();
        }

        // POST: MedicalShopProductStocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MedicalShopId,ProductId,TotalQuantity")] MedicalShopProductStock medicalShopProductStock)
        {
                _context.Add(medicalShopProductStock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }

        // GET: MedicalShopProductStocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "MedicalShopRegister");
            }
            if (id == null || _context.MedicalShopProductStocks == null)
            {
                return NotFound();
            }

            var medicalShopProductStock = await _context.MedicalShopProductStocks.FindAsync(id);
            if (medicalShopProductStock == null)
            {
                return NotFound();
            }
            ViewBag.MedicalShopId = new SelectList(_context.MedicalShopDetails, "Id", "OwnerName");
            ViewBag.ProductId = new SelectList(_context.ProductDetails, "Id", "ProductName");
            return View(medicalShopProductStock);
        }

        // POST: MedicalShopProductStocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MedicalShopId,ProductId,TotalQuantity")] MedicalShopProductStock medicalShopProductStock)
        {
            if (id != medicalShopProductStock.Id)
            {
                return NotFound();
            }

                try
                {
                    _context.Update(medicalShopProductStock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalShopProductStockExists(medicalShopProductStock.Id))
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

        // GET: MedicalShopProductStocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "MedicalShopRegister");
            }
            if (id == null || _context.MedicalShopProductStocks == null)
            {
                return NotFound();
            }

            var medicalShopProductStock = await _context.MedicalShopProductStocks
                .Include(m => m.MedicalShop)
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalShopProductStock == null)
            {
                return NotFound();
            }

            return View(medicalShopProductStock);
        }

        // POST: MedicalShopProductStocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MedicalShopProductStocks == null)
            {
                return Problem("Entity set 'pharma_managementContext.MedicalShopProductStocks'  is null.");
            }
            var medicalShopProductStock = await _context.MedicalShopProductStocks.FindAsync(id);
            if (medicalShopProductStock != null)
            {
                _context.MedicalShopProductStocks.Remove(medicalShopProductStock);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalShopProductStockExists(int id)
        {
          return (_context.MedicalShopProductStocks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
