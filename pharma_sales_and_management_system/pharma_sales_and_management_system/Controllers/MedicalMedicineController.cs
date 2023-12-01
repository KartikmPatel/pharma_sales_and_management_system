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
    public class MedicalMedicineController : Controller
    {
        private readonly pharma_managementContext _context;

        public MedicalMedicineController(pharma_managementContext context)
        {
            _context = context;
        }

        // GET: MedicalMedicine
        public async Task<IActionResult> Index()
        {
            var pharma_managementContext = _context.ProductDetails.Include(p => p.Category).Include(p => p.Company);
            return View(await pharma_managementContext.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Id,ProductName,RetailPrice,ProductImage,Description,Disease,CategoryId,MfgDate,CompanyId,ExpDate")] ProductDetail productDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Id", productDetail.CategoryId);
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id", productDetail.CompanyId);
            return View(productDetail);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,RetailPrice,ProductImage,Description,Disease,CategoryId,MfgDate,CompanyId,ExpDate")] ProductDetail productDetail)
        {
            if (id != productDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "Id", "Id", productDetail.CategoryId);
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id", productDetail.CompanyId);
            return View(productDetail);
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
    }
}
