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
    public class AgencyOrdersController : Controller
    {
        private readonly pharma_managementContext _context;

        public AgencyOrdersController(pharma_managementContext context)
        {
            _context = context;
        }

        private bool IsUserAuthenticated()
        {
            return HttpContext.Session.GetInt32("AgencyId").HasValue;
        }

        // GET: AgencyOrders
        public async Task<IActionResult> Index()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            var agencyDetails = await (from i in _context.AgencyDetails
                                       select i).FirstOrDefaultAsync();
            ViewBag.ProfilePhoto = agencyDetails.ProfileImage;
            ViewBag.editId = agencyDetails.Id;
            var pharma_managementContext = _context.AgencyOrders.Include(a => a.Agency).Include(a => a.Company).Include(a => a.Product);
            ViewBag.Success = TempData["success"];
            return View(await pharma_managementContext.ToListAsync());
        }

        // GET: AgencyOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            if (id == null || _context.AgencyOrders == null)
            {
                return NotFound();
            }

            var agencyOrder = await _context.AgencyOrders
                .Include(a => a.Agency)
                .Include(a => a.Company)
                .Include(a => a.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (agencyOrder == null)
            {
                return NotFound();
            }

            return View(agencyOrder);
        }

        // GET: AgencyOrders/Create
        public IActionResult Create()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            var agencyDetails = (from i in _context.AgencyDetails
                                      select i).FirstOrDefault();
            ViewBag.ProfilePhoto = agencyDetails.ProfileImage;
            ViewBag.editId = agencyDetails.Id;
            ViewBag.ProductId = new SelectList(_context.ProductDetails, "Id", "ProductName");
            ViewBag.AgencyId = new SelectList(_context.AgencyDetails, "Id", "AgencyName");
            ViewBag.CompanyId = new SelectList(_context.Manufacturers, "Id", "ComponyName");
            return View();
        }

        // POST: AgencyOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Quantity,AgencyId,CompanyId,IsDelivered")] AgencyOrder agencyOrder)
        {
            agencyOrder.Date = DateTime.Now;
                _context.Add(agencyOrder);
                await _context.SaveChangesAsync();
                TempData["success"] = "Order Successfully Placed";
                return RedirectToAction(nameof(Index));
        }

        // GET: AgencyOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            if (id == null || _context.AgencyOrders == null)
            {
                return NotFound();
            }

            var agencyOrder = await _context.AgencyOrders.FindAsync(id);
            if (agencyOrder == null)
            {
                return NotFound();
            }
            ViewData["AgencyId"] = new SelectList(_context.AgencyDetails, "Id", "Id", agencyOrder.AgencyId);
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id", agencyOrder.CompanyId);
            ViewData["ProductId"] = new SelectList(_context.ProductDetails, "Id", "Id", agencyOrder.ProductId);
            return View(agencyOrder);
        }

        // POST: AgencyOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Quantity,Date,AgencyId,CompanyId,IsDelivered")] AgencyOrder agencyOrder)
        {
            if (id != agencyOrder.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agencyOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgencyOrderExists(agencyOrder.Id))
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
            ViewData["AgencyId"] = new SelectList(_context.AgencyDetails, "Id", "Id", agencyOrder.AgencyId);
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id", agencyOrder.CompanyId);
            ViewData["ProductId"] = new SelectList(_context.ProductDetails, "Id", "Id", agencyOrder.ProductId);
            return View(agencyOrder);
        }

        // GET: AgencyOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            if (id == null || _context.AgencyOrders == null)
            {
                return NotFound();
            }

            var agencyOrder = await _context.AgencyOrders
                .Include(a => a.Agency)
                .Include(a => a.Company)
                .Include(a => a.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (agencyOrder == null)
            {
                return NotFound();
            }

            return View(agencyOrder);
        }

        // POST: AgencyOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AgencyOrders == null)
            {
                return Problem("Entity set 'pharma_managementContext.AgencyOrders'  is null.");
            }
            var agencyOrder = await _context.AgencyOrders.FindAsync(id);
            if (agencyOrder != null)
            {
                _context.AgencyOrders.Remove(agencyOrder);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgencyOrderExists(int id)
        {
          return (_context.AgencyOrders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
