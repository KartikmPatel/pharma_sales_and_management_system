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
    public class AgencyController : Controller
    {
        private readonly pharma_managementContext _context;

        public AgencyController(pharma_managementContext context)
        {
            _context = context;
        }

        // GET: Agency
        public async Task<IActionResult> Index()
        {
              return _context.AgencyDetails != null ? 
                          View(await _context.AgencyDetails.ToListAsync()) :
                          Problem("Entity set 'pharma_managementContext.AgencyDetails'  is null.");
        }

        // GET: Agency/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AgencyDetails == null)
            {
                return NotFound();
            }

            var agencyDetail = await _context.AgencyDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (agencyDetail == null)
            {
                return NotFound();
            }

            return View(agencyDetail);
        }

        // GET: Agency/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Agency/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AgencyName,Email,ContactNo,Password,ProfileImage")] AgencyDetail agencyDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(agencyDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(agencyDetail);
        }

        // GET: Agency/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AgencyDetails == null)
            {
                return NotFound();
            }

            var agencyDetail = await _context.AgencyDetails.FindAsync(id);
            if (agencyDetail == null)
            {
                return NotFound();
            }
            return View(agencyDetail);
        }

        // POST: Agency/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AgencyName,Email,ContactNo,Password,ProfileImage")] AgencyDetail agencyDetail)
        {
            if (id != agencyDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agencyDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgencyDetailExists(agencyDetail.Id))
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
            return View(agencyDetail);
        }

        // GET: Agency/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AgencyDetails == null)
            {
                return NotFound();
            }

            var agencyDetail = await _context.AgencyDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (agencyDetail == null)
            {
                return NotFound();
            }

            return View(agencyDetail);
        }

        // POST: Agency/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AgencyDetails == null)
            {
                return Problem("Entity set 'pharma_managementContext.AgencyDetails'  is null.");
            }
            var agencyDetail = await _context.AgencyDetails.FindAsync(id);
            if (agencyDetail != null)
            {
                _context.AgencyDetails.Remove(agencyDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgencyDetailExists(int id)
        {
          return (_context.AgencyDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
