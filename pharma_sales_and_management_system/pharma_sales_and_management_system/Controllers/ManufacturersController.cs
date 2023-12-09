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
    public class ManufacturersController : Controller
    {
        private readonly pharma_managementContext _context;

        public ManufacturersController(pharma_managementContext context)
        {
            _context = context;
        }

        private bool IsUserAuthenticated()
        {
            return HttpContext.Session.GetInt32("ManufacturerId").HasValue;
        }

        // GET: Manufacturers
        public async Task<IActionResult> Index(string search)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                var manufacturerId = HttpContext.Session.GetInt32("ManufacturerId");

                if (!manufacturerId.HasValue)
                {
                    return RedirectToAction(nameof(Login));
                }

                var manufacturerDetail = await _context.Manufacturers.FirstOrDefaultAsync(m => m.Id == manufacturerId.Value);

                if (manufacturerDetail != null)
                {
                    if (search != null)
                    {
                        var searchData = from m in _context.Manufacturers
                                         where m.ComponyName.Contains(search) || m.Email.Contains(search) || m.ContactNo.ToString().Contains(search) || m.City.Contains(search)
                                         select m;
                        return View(await searchData.ToListAsync());
                    }
                    return View(new List<Manufacturer> { manufacturerDetail });
                }
                return Problem("Entity set 'pharma_managementContext.Manufacturers'  is null.");
            }
        }

        // GET: Manufacturers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }

            if (id == null || _context.Manufacturers == null)
            {
                return NotFound();
            }

            var manufacturer = await _context.Manufacturers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manufacturer == null)
            {
                return NotFound();
            }

            return View(manufacturer);
        }

        // GET: Manufacturers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Manufacturers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ComponyName,Email,ContactNo,Password,City")] Manufacturer manufacturer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(manufacturer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(manufacturer);
        }

        // GET: Manufacturers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }

            if (id == null || _context.Manufacturers == null)
            {
                return NotFound();
            }

            var manufacturer = await _context.Manufacturers.FindAsync(id);
            if (manufacturer == null)
            {
                return NotFound();
            }
            return View(manufacturer);
        }

        // POST: Manufacturers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ComponyName,Email,ContactNo,Password,City")] Manufacturer manufacturer)
        {
            if (id != manufacturer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(manufacturer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManufacturerExists(manufacturer.Id))
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
            return View(manufacturer);
        }

        // GET: Manufacturers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction(nameof(Login));
            }

            if (id == null || _context.Manufacturers == null)
            {
                return NotFound();
            }

            var manufacturer = await _context.Manufacturers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manufacturer == null)
            {
                return NotFound();
            }

            return View(manufacturer);
        }

        // POST: Manufacturers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Manufacturers == null)
            {
                return Problem("Entity set 'pharma_managementContext.Manufacturers'  is null.");
            }
            var manufacturer = await _context.Manufacturers.FindAsync(id);
            if (manufacturer != null)
            {
                _context.Manufacturers.Remove(manufacturer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ManufacturerExists(int id)
        {
          return (_context.Manufacturers?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Id,Email,Password")] Manufacturer manufacturer)
        {
            var manu = _context.Manufacturers.FirstOrDefault(u => u.Email == manufacturer.Email && u.Password == manufacturer.Password);

            if (manu != null)
            {
                // Store user's Id in session
                HttpContext.Session.SetInt32("ManufacturerId", manu.Id);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Login));
            }
        }

        public IActionResult Logout()
        {
            // Clear user's session data
            HttpContext.Session.Remove("ManufacturerId");

            // Redirect to the login page or any other page after logout
            return RedirectToAction(nameof(Login));
        }
    }
}
