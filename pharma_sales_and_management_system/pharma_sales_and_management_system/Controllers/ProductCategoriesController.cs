using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pharma_sales_and_management_system.Models;

namespace pharma_sales_and_management_system.Controllers
{
    public class ProductCategoriesController : Controller
    {
        private readonly pharma_managementContext _context;

        public ProductCategoriesController(pharma_managementContext context)
        {
            _context = context;
        }

        private bool IsUserAuthenticated()
        {
            return HttpContext.Session.GetInt32("AgencyId").HasValue;
        }

        // GET: ProductCategories
        public async Task<IActionResult> Index(string search)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            if (search != null)
            {
                var searchCategory = from c in _context.ProductCategories
                                    where c.CategoryName.Contains(search)
                                    select c;
                return View(await searchCategory.ToListAsync());
            }

            var agencyDetails = await (from i in _context.AgencyDetails
                                        select i).FirstOrDefaultAsync();
            ViewBag.ProfilePhoto = agencyDetails.ProfileImage;
            ViewBag.editId = agencyDetails.Id;

            ViewBag.Success = TempData["success"];
            return _context.ProductCategories != null ? 
                          View(await _context.ProductCategories.ToListAsync()) :
                          Problem("Entity set 'pharma_managementContext.ProductCategories'  is null.");
        }

        // GET: ProductCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            if (id == null || _context.ProductCategories == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            var agencyDetails = await (from i in _context.AgencyDetails
                                       select i).FirstOrDefaultAsync();
            ViewBag.ProfilePhoto = agencyDetails.ProfileImage;
            ViewBag.editId = agencyDetails.Id;
            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }

        // GET: ProductCategories/Create
        public IActionResult Create()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            var agencyDetails = (from i in _context.AgencyDetails
                                      select i).FirstOrDefault();
            ViewBag.ProfilePhoto = agencyDetails.ProfileImage;
            return View();
        }

        // POST: ProductCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryName")] ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            TempData["success"] = "Category Successfully Added";
            return View(productCategory);
        }

        // GET: ProductCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            if (id == null || _context.ProductCategories == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategories.FindAsync(id);
            var agencyDetails = await (from i in _context.AgencyDetails
                                       select i).FirstOrDefaultAsync();
            ViewBag.ProfilePhoto = agencyDetails.ProfileImage;
            ViewBag.editId = agencyDetails.Id;
            if (productCategory == null)
            {
                return NotFound();
            }
            return View(productCategory);
        }

        // POST: ProductCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryName")] ProductCategory productCategory)
        {
            if (id != productCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductCategoryExists(productCategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"] = "Category Successfully Edited";
                return RedirectToAction(nameof(Index));
            }
            return View(productCategory);
        }

        // GET: ProductCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            if (id == null || _context.ProductCategories == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            var agencyDetails = await (from i in _context.AgencyDetails
                                       select i).FirstOrDefaultAsync();
            ViewBag.ProfilePhoto = agencyDetails.ProfileImage;
            ViewBag.editId = agencyDetails.Id;
            if (productCategory == null)
            {
                return NotFound();
            }

            return View(productCategory);
        }

        // POST: ProductCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductCategories == null)
            {
                return Problem("Entity set 'pharma_managementContext.ProductCategories'  is null.");
            }
            var productCategory = await _context.ProductCategories.FindAsync(id);
            if (productCategory != null)
            {
                _context.ProductCategories.Remove(productCategory);
            }
            
            await _context.SaveChangesAsync();
            TempData["success"] = "Category Successfully Deleted";
            return RedirectToAction(nameof(Index));
        }

        private bool ProductCategoryExists(int id)
        {
          return (_context.ProductCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
