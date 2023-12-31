﻿using System;
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
    public class MedicalShopRegisterController : Controller
    {
        private readonly pharma_managementContext _context;
        private readonly IWebHostEnvironment _webHostEnv;

        public MedicalShopRegisterController(pharma_managementContext context, IWebHostEnvironment webHostEnv)
        {
            _context = context;
            _webHostEnv = webHostEnv;
        }

        private bool IsUserAuthenticated()
        {
            return HttpContext.Session.GetInt32("MedicalShopId").HasValue;
        }

        // GET: MedicalShopRegister
        public async Task<IActionResult> Index(string search)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                var medicalShopId = HttpContext.Session.GetInt32("MedicalShopId");

                if (!medicalShopId.HasValue)
                {
                    return RedirectToAction(nameof(Login));
                }

                var medicalShopDetail = await _context.MedicalShopDetails.FirstOrDefaultAsync(m => m.Id == medicalShopId.Value);

                if (medicalShopDetail != null)
                {
                    ViewBag.ProfilePhoto = medicalShopDetail.ProfilePic;
                    ViewBag.editId = medicalShopDetail.Id;
                    if (search != null)
                    {
                        var searchResults = new List<MedicalShopDetail>
                {
                    medicalShopDetail
                }.Where(m => m.OwnerName.Contains(search) || m.Email.Contains(search) || m.ContactNo.ToString().Contains(search) || m.City.Contains(search) || m.ProfilePic.Contains(search));

                        return View(searchResults.ToList());
                    }
                    return View(new List<MedicalShopDetail> { medicalShopDetail });
                }
                return Problem("Entity set 'pharma_managementContext.MedicalShopDetails' is null.");
            }
        }

        public IActionResult UserFeedback()
        {
            var medicalShopId = HttpContext.Session.GetInt32("MedicalShopId");
            if (!medicalShopId.HasValue)
            {
                return RedirectToAction(nameof(Login));
            }
            var medicalShopDetail1 = (from m in _context.MedicalShopDetails
                                      where m.Id == medicalShopId
                                      select m).FirstOrDefault();
            ViewBag.ProfilePhoto = medicalShopDetail1.ProfilePic;
            ViewBag.editId = medicalShopDetail1.Id;

            //var feedbackdata = (from f in _context.Feedbacks
            //                   where f.MedicalShopId == medicalShopId
            //                   select f).ToList();
            var feedbackdata = _context.Feedbacks.Where(f => f.MedicalShopId == medicalShopId).Include(f => f.MedicalShop).Include(f => f.User);
            return View(feedbackdata);

        }

        // GET: MedicalShopRegister/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }

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
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }

            if (id == null || _context.MedicalShopDetails == null)
            {
                return NotFound();
            }

            var medicalShopDetail = await _context.MedicalShopDetails.FindAsync(id);
            if (medicalShopDetail == null)
            {
                return NotFound();
            }

            var medicalShopId = HttpContext.Session.GetInt32("MedicalShopId");
            var medicalShopDetail1 = await _context.MedicalShopDetails.FirstOrDefaultAsync(m => m.Id == medicalShopId.Value);
                ViewBag.ProfilePhoto = medicalShopDetail1.ProfilePic;
                ViewBag.editId = medicalShopDetail1.Id;
            ViewBag.successmessage = TempData["success"];
                return View(medicalShopDetail);
        }

        // POST: MedicalShopRegister/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OwnerName,Email,ContactNo,City,Password,ProfilePic,IsConfirmed")] MedicalShopDetail medicalShopDetail, IFormFile file,string oldfile)
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
                TempData["success"] = "Profile Successfully Edited";
                return RedirectToAction(nameof(Edit));
        }

        // GET: MedicalShopRegister/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }

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

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Id,Email,Password")] MedicalShopDetail medicalShopDetail)
        {
            var medicalShop = _context.MedicalShopDetails.FirstOrDefault(u => u.Email == medicalShopDetail.Email && u.Password == medicalShopDetail.Password);

            if (medicalShop != null)
            {
                // Store user's Id in session
                HttpContext.Session.SetInt32("MedicalShopId", medicalShop.Id);

                return RedirectToAction("Index","MedicalMedicine");
            }
            else
            {
                TempData["InvalidLogin"] = "Invalid Username OR Password";
                return RedirectToAction(nameof(Login));
            }
        }

        public IActionResult Logout()
        {
            // Clear user's session data
            HttpContext.Session.Remove("MedicalShopId");

            // Redirect to the login page or any other page after logout
            return RedirectToAction(nameof(Login));
        }
    }
}
