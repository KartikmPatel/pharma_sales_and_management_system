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
    public class AgencyOrdersConfirmController : Controller
    {
        private readonly pharma_managementContext _context;

        public AgencyOrdersConfirmController(pharma_managementContext context)
        {
            _context = context;
        }

        private bool IsUserAuthenticated()
        {
            return HttpContext.Session.GetInt32("ManufacturerId").HasValue;
        }

        // GET: AgencyOrdersConfirm
        public async Task<IActionResult> Index()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Manufacturers");
            }
            else
            {
                var manufacturerId = HttpContext.Session.GetInt32("ManufacturerId");

                if (!manufacturerId.HasValue)
                {
                    return RedirectToAction("Login", "Manufacturers");
                }
            
                var mid = await (from m in _context.Manufacturers
                                               where m.Id == manufacturerId.Value
                                               select m.Id).FirstOrDefaultAsync();
                ViewBag.editId = mid;
                if (mid != 0)
                {
                    var pharma_managementContext = await (from a in _context.AgencyOrders
                                        where a.CompanyId == mid
                                        select a).Include(a => a.Agency).Include(a => a.Company).Include(a => a.Product).ToListAsync();
                    return View(pharma_managementContext);
                }
            }
            return Problem("Entity set 'pharma_managementContext.Manufacturers'  is null.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string verify,int qty,int pid, int id, AgencyOrder AgencyOrderDetail)
        {
            if (id != AgencyOrderDetail.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            try
            {
                //var verify1 = Convert.ToInt32(verify);
                if (verify == "0")
                {
                    AgencyOrderDetail.IsDelivered = 1; // Assuming IsConfirmed is an integer property
                    _context.Entry(AgencyOrderDetail).Property(x => x.IsDelivered).IsModified = true;
                    await _context.SaveChangesAsync();
                    if (qty > 0)
                    {
                        var productStock = await _context.AgencyProductStocks.FirstOrDefaultAsync(o => o.ProductId == pid);

                        if (productStock != null)
                        {
                            int totalQuantity = productStock.TotalQuantity + qty;
                            productStock.TotalQuantity = totalQuantity;

                            _context.Entry(productStock).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //if (!AgencyOrderExists(agencyOrder.Id))
                //{
                //    return NotFound();
                //}
                //else
                //{
                //    throw;
                //}
            }
            return RedirectToAction(nameof(Index));
            //}
            //return View(medicalShopDetail);
        }

        private bool AgencyOrderExists(object id)
        {
            throw new NotImplementedException();
        }

        // GET: AgencyOrdersConfirm/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Manufacturers");
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

        // GET: AgencyOrdersConfirm/Create
        public IActionResult Create()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Manufacturers");
            }
            ViewData["AgencyId"] = new SelectList(_context.AgencyDetails, "Id", "Id");
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.ProductDetails, "Id", "Id");
            return View();
        }

        // POST: AgencyOrdersConfirm/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Quantity,Date,AgencyId,CompanyId,IsDelivered")] AgencyOrder agencyOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(agencyOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AgencyId"] = new SelectList(_context.AgencyDetails, "Id", "Id", agencyOrder.AgencyId);
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id", agencyOrder.CompanyId);
            ViewData["ProductId"] = new SelectList(_context.ProductDetails, "Id", "Id", agencyOrder.ProductId);
            return View(agencyOrder);
        }

        // GET: AgencyOrdersConfirm/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Manufacturers");
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

        // POST: AgencyOrdersConfirm/Edit/5
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

        // GET: AgencyOrdersConfirm/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Manufacturers");
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

        // POST: AgencyOrdersConfirm/Delete/5
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
