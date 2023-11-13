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
    public class MedicalController : Controller
    {
        private readonly pharma_managementContext _context;

        public MedicalController(pharma_managementContext context)
        {
            _context = context;
        }

        // GET: Medical
        public async Task<IActionResult> Index()
        {
              return _context.MedicalShopDetails != null ? 
                          View(await _context.MedicalShopDetails.ToListAsync()) :
                          Problem("Entity set 'pharma_managementContext.MedicalShopDetails'  is null.");
        }

        // GET: Medical/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MedicalShopDetails == null)
            {
                return NotFound();
            }

            var medicalShopDetail = await _context.MedicalShopDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalShopDetail == null)
            {
                return NotFound();
            }

            return View(medicalShopDetail);
        }

        // GET: Medical/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Medical/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OwnerName,Email,ContactNo,City,Password,ProfilePic")] MedicalShopDetail medicalShopDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicalShopDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(medicalShopDetail);
        }

        // GET: Medical/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MedicalShopDetails == null)
            {
                return NotFound();
            }

            var medicalShopDetail = await _context.MedicalShopDetails.FindAsync(id);
            if (medicalShopDetail == null)
            {
                return NotFound();
            }
            return View(medicalShopDetail);
        }

        // POST: Medical/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OwnerName,Email,ContactNo,City,Password,ProfilePic")] MedicalShopDetail medicalShopDetail)
        {
            if (id != medicalShopDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalShopDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalShopDetailExists(medicalShopDetail.Id))
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
            return View(medicalShopDetail);
        }

        // GET: Medical/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MedicalShopDetails == null)
            {
                return NotFound();
            }

            var medicalShopDetail = await _context.MedicalShopDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalShopDetail == null)
            {
                return NotFound();
            }

            return View(medicalShopDetail);
        }

        // POST: Medical/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MedicalShopDetails == null)
            {
                return Problem("Entity set 'pharma_managementContext.MedicalShopDetails'  is null.");
            }
            var medicalShopDetail = await _context.MedicalShopDetails.FindAsync(id);
            if (medicalShopDetail != null)
            {
                _context.MedicalShopDetails.Remove(medicalShopDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalShopDetailExists(int id)
        {
          return (_context.MedicalShopDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
