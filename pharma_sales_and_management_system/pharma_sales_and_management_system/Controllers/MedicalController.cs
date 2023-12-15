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
        private readonly IWebHostEnvironment _webHostEnv;

        public MedicalController(pharma_managementContext context, IWebHostEnvironment webHostEnv)
        {
            _context = context;
            _webHostEnv = webHostEnv;
        }

        // GET: Medical
        public async Task<IActionResult> Index(string search)
        {
            if (search != null)
            {
                var searchMedical = from m in _context.MedicalShopDetails
                                 where m.OwnerName.Contains(search) || m.Email.Contains(search) || m.ContactNo.ToString().Contains(search) || m.City.Contains(search)
                                 select m;
                return View(await searchMedical.ToListAsync());
            }
            return _context.MedicalShopDetails != null ? 
                          View(await _context.MedicalShopDetails.ToListAsync()) :
                          Problem("Entity set 'pharma_managementContext.MedicalShopDetails'  is null.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string verify, int id,MedicalShopDetail medicalShopDetail)
        {
            if (id != medicalShopDetail.Id)
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
                    medicalShopDetail.IsConfirmed = 1; // Assuming IsConfirmed is an integer property
                    _context.Entry(medicalShopDetail).Property(x => x.IsConfirmed).IsModified = true;
                    await _context.SaveChangesAsync();
                }
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
            //}
            //return View(medicalShopDetail);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(string verify, int id,MedicalShopDetail medicalShopDetail)
        {
            if (id != medicalShopDetail.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                var verify1 = Convert.ToInt32(verify);
                if (verify1 == 0)
                {
                    _context.Update(medicalShopDetail.IsConfirmed = 1);
                    await _context.SaveChangesAsync();
                }
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
                //return RedirectToAction(nameof(Index));
            //}
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,OwnerName,Email,ContactNo,City,Password,ProfilePic,IsConfirmed")] MedicalShopDetail medicalShopDetail, IFormFile file, string oldfile)
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
                    else if (!string.IsNullOrEmpty(oldfile))
                    {
                        medicalShopDetail.ProfilePic = oldfile;
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
