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
    public class AgencyProductStocksController : Controller
    {
        private readonly pharma_managementContext _context;

        public AgencyProductStocksController(pharma_managementContext context)
        {
            _context = context;
        }

        private bool IsUserAuthenticated()
        {
            return HttpContext.Session.GetInt32("AgencyId").HasValue;
        }

        // GET: AgencyProductStocks
        public async Task<IActionResult> Index()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            var agencyDetails = await (from i in _context.AgencyDetails
                                       select i).FirstOrDefaultAsync();
            ViewBag.ProfilePhoto = agencyDetails.ProfileImage;
            var pharma_managementContext = _context.AgencyProductStocks.Include(p => p.Product);
            return View(await pharma_managementContext.ToListAsync());                
        }

        // GET: AgencyProductStocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            if (id == null || _context.AgencyProductStocks == null)
            {
                return NotFound();
            }

            var agencyProductStock = await _context.AgencyProductStocks.Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (agencyProductStock == null)
            {
                return NotFound();
            }

            return View(agencyProductStock);
        }

        // GET: AgencyProductStocks/Create
        public IActionResult Create()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            ViewBag.ProductId = new SelectList(_context.ProductDetails, "Id", "ProductName");
            return View();
        }

        // POST: AgencyProductStocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TotalQuantity,ProductId")] AgencyProductStock agencyProductStock)
        {
                _context.Add(agencyProductStock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }

        // GET: AgencyProductStocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            if (id == null || _context.AgencyProductStocks == null)
            {
                return NotFound();
            }

            var agencyProductStock = await _context.AgencyProductStocks.FindAsync(id);
            if (agencyProductStock == null)
            {
                return NotFound();
            }

            ViewBag.ProductId = new SelectList(_context.ProductDetails, "Id", "ProductName");
            return View(agencyProductStock);
        }

        // POST: AgencyProductStocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TotalQuantity,ProductId")] AgencyProductStock agencyProductStock)
        {
            if (id != agencyProductStock.Id)
            {
                return NotFound();
            }

            
                try
                {
                    _context.Update(agencyProductStock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgencyProductStockExists(agencyProductStock.Id))
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

        // GET: AgencyProductStocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            if (id == null || _context.AgencyProductStocks == null)
            {
                return NotFound();
            }

            var agencyProductStock = await _context.AgencyProductStocks.Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (agencyProductStock == null)
            {
                return NotFound();
            }

            return View(agencyProductStock);
        }

        // POST: AgencyProductStocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AgencyProductStocks == null)
            {
                return Problem("Entity set 'pharma_managementContext.AgencyProductStocks'  is null.");
            }
            var agencyProductStock = await _context.AgencyProductStocks.FindAsync(id);
            if (agencyProductStock != null)
            {
                _context.AgencyProductStocks.Remove(agencyProductStock);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgencyProductStockExists(int id)
        {
          return (_context.AgencyProductStocks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
