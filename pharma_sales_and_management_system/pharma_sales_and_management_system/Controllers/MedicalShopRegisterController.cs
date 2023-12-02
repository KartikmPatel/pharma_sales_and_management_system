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
    public class MedicalShopRegisterController : Controller
    {
        private readonly pharma_managementContext _context;
        private readonly IWebHostEnvironment _webHostEnv;

        public MedicalShopRegisterController(pharma_managementContext context, IWebHostEnvironment webHostEnv)
        {
            _context = context;
            _webHostEnv = webHostEnv;
        }

        // GET: MedicalShopRegister
        public async Task<IActionResult> Index()
        {
              return _context.MedicalShopDetails != null ? 
                          View(await _context.MedicalShopDetails.ToListAsync()) :
                          Problem("Entity set 'pharma_managementContext.MedicalShopDetails'  is null.");
        }

        // GET: MedicalShopRegister/Details/5
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

        // GET: MedicalShopRegister/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MedicalShopRegister/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OwnerName,Email,ContactNo,City,Password,ProfilePic,IsConfirmed")] MedicalShopDetail medicalShopDetail, IFormFile file)
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
                medicalShopDetail.ProfilePic = @"\images\user\" + filename;
            }
            _context.Add(medicalShopDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); 
        }

        // GET: MedicalShopRegister/Edit/5
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

        // POST: MedicalShopRegister/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OwnerName,Email,ContactNo,City,Password,ProfilePic,IsConfirmed")] MedicalShopDetail medicalShopDetail, IFormFile file)
        {
            if (id != medicalShopDetail.Id)
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
                    medicalShopDetail.ProfilePic = @"\images\user\" + filename;
                }
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

        // GET: MedicalShopRegister/Delete/5
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

        // POST: MedicalShopRegister/Delete/5
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
