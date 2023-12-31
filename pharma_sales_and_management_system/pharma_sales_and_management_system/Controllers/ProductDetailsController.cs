﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pharma_sales_and_management_system.Models;

namespace pharma_sales_and_management_system.Controllers
{
    public class ProductDetailsController : Controller
    {
        private readonly pharma_managementContext _context;
        private readonly IWebHostEnvironment _webHostEnv;

        public ProductDetailsController(pharma_managementContext context, IWebHostEnvironment webHostEnv)
        {
            _context = context;
            _webHostEnv = webHostEnv;
        }

        private bool IsUserAuthenticated()
        {
            return HttpContext.Session.GetInt32("AgencyId").HasValue;
        }

        // GET: ProductDetails
        public async Task<IActionResult> Index()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            var pharma_managementContext = _context.ProductDetails.Include(p => p.Category).Include(p => p.Company);
            var agencyDetails = await (from i in _context.AgencyDetails
                                       select i).FirstOrDefaultAsync();
            ViewBag.ProfilePhoto = agencyDetails.ProfileImage;
            ViewBag.editId = agencyDetails.Id;
            ViewBag.Success = TempData["success"];
            return View(await pharma_managementContext.ToListAsync());
        }

        // GET: ProductDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            if (id == null || _context.ProductDetails == null)
            {
                return NotFound();
            }

            var productDetail = await _context.ProductDetails
                .Include(p => p.Category)
                .Include(p => p.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            var agencyDetails = await (from i in _context.AgencyDetails
                                       select i).FirstOrDefaultAsync();
            ViewBag.ProfilePhoto = agencyDetails.ProfileImage;
            ViewBag.editId = agencyDetails.Id;
            if (productDetail == null)
            {
                return NotFound();
            }

            return View(productDetail);
        }

        // GET: ProductDetails/Create
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
            ViewBag.CategoryId = new SelectList(_context.ProductCategories, "Id", "CategoryName");
            ViewBag.CompanyId = new SelectList(_context.Manufacturers, "Id", "ComponyName");
            return View();
        }

        // POST: ProductDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductName,RetailPrice,ProductImage,Description,Disease,CategoryId,MfgDate,CompanyId,ExpDate")] ProductDetail productDetail, IFormFile file,AgencyProductStock stockDetails)
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

            if (productDetail.Id != 0)
            {
            var pid = productDetail.Id;
            stockDetails.ProductId = pid;
            stockDetails.TotalQuantity = 0;
            _context.Add(stockDetails);
            await _context.SaveChangesAsync();
            }

            TempData["success"] = "Product Successfully Added";
            return RedirectToAction(nameof(Index));
            
            //return View(productDetail);
        }

        // GET: ProductDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            if (id == null || _context.ProductDetails == null)
            {
                return NotFound();
            }

            var productDetail = await _context.ProductDetails.FindAsync(id);
            var agencyDetails = await (from i in _context.AgencyDetails
                                       select i).FirstOrDefaultAsync();
            ViewBag.ProfilePhoto = agencyDetails.ProfileImage;
            ViewBag.editId = agencyDetails.Id;
            if (productDetail == null)
            {
                return NotFound();
            }
            ViewBag.CategoryId = new SelectList(_context.ProductCategories, "Id", "CategoryName");
            ViewBag.CompanyId = new SelectList(_context.Manufacturers, "Id", "ComponyName");
            return View(productDetail);
        }

        // POST: ProductDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,RetailPrice,ProductImage,Description,Disease,CategoryId,MfgDate,CompanyId,ExpDate")] ProductDetail productDetail, IFormFile file, string oldfile)
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
            TempData["success"] = "Product Successfully Edited";
            return RedirectToAction(nameof(Index));
        }


        // GET: ProductDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Agency");
            }
            if (id == null || _context.ProductDetails == null)
            {
                return NotFound();
            }

            var productDetail = await _context.ProductDetails
                .Include(p => p.Category)
                .Include(p => p.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            var agencyDetails = await (from i in _context.AgencyDetails
                                       select i).FirstOrDefaultAsync();
            ViewBag.ProfilePhoto = agencyDetails.ProfileImage;
            ViewBag.editId = agencyDetails.Id;
            if (productDetail == null)
            {
                return NotFound();
            }

            return View(productDetail);
        }

        // POST: ProductDetails/Delete/5
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
            
            var sid = await (from s in _context.AgencyProductStocks
                      where s.ProductId == id
                      select s).FirstOrDefaultAsync();
            
            if (sid != null)
            {
                _context.AgencyProductStocks.Remove(sid);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Product Successfully Deleted";
            return RedirectToAction(nameof(Index));
        }

        private bool ProductDetailExists(int id)
        {
          return (_context.ProductDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
