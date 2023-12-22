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
    public class FeedbacksController : Controller
    {
        private readonly pharma_managementContext _context;

        public FeedbacksController(pharma_managementContext context)
        {
            _context = context;
        }

        private bool IsUserAuthenticated()
        {
            return HttpContext.Session.GetInt32("UserId").HasValue;
        }

        // GET: Feedbacks
        public async Task<IActionResult> Index()
        {
            var UserId = HttpContext.Session.GetInt32("UserId");
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login","User");
            }
            var uDetails = await (from u in _context.UserDetails
                                  where u.Id == UserId
                                  select u).FirstOrDefaultAsync();
            ViewBag.ProfilePhoto = uDetails.ProfilePic;
            ViewBag.editId = uDetails.Id;
            var pharma_managementContext = _context.Feedbacks.Include(f => f.MedicalShop).Include(f => f.User);
            return View(await pharma_managementContext.ToListAsync());
        }

        // GET: Feedbacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var UserId = HttpContext.Session.GetInt32("UserId");
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "User");
            }
            var uDetails = await (from u in _context.UserDetails
                                  where u.Id == UserId
                                  select u).FirstOrDefaultAsync();
            ViewBag.ProfilePhoto = uDetails.ProfilePic;
            ViewBag.editId = uDetails.Id;
            if (id == null || _context.Feedbacks == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.MedicalShop)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // GET: Feedbacks/Create
        public IActionResult Create()
        {
            var UserId = HttpContext.Session.GetInt32("UserId");
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "User");
            }
            var uDetails = (from u in _context.UserDetails
                                 where u.Id == UserId
                                 select u).FirstOrDefault();
            ViewBag.ProfilePhoto = uDetails.ProfilePic;
            ViewBag.editId = uDetails.Id;
            ViewBag.UserName = uDetails.Name;
            ViewBag.OwnerName = new SelectList(_context.MedicalShopDetails, "Id", "OwnerName");
            ViewBag.successmessage = TempData["success"];
            return View();
        }

        // POST: Feedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,UserId,MedicalShopId")] Feedback feedback,string username)
        {
                var userId = await (from u in _context.UserDetails
                                    where u.Name == username
                                    select u.Id).FirstOrDefaultAsync();
                feedback.UserId = userId;
                _context.Add(feedback);
                await _context.SaveChangesAsync();

            TempData["success"] = "Feedback Successfully send";
            return RedirectToAction(nameof(Create));
        }

        // GET: Feedbacks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var UserId = HttpContext.Session.GetInt32("UserId");
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "User");
            }
            var uDetails = await (from u in _context.UserDetails
                                  where u.Id == UserId
                                  select u).FirstOrDefaultAsync();
            ViewBag.ProfilePhoto = uDetails.ProfilePic;
            ViewBag.editId = uDetails.Id;
            if (id == null || _context.Feedbacks == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            ViewBag.OwnerName = new SelectList(_context.MedicalShopDetails, "Id", "OwnerName");
            ViewBag.Name = new SelectList(_context.UserDetails, "Id", "Name");
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,UserId,MedicalShopId")] Feedback feedback)
        {
            if (id != feedback.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.Id))
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
            ViewData["MedicalShopId"] = new SelectList(_context.MedicalShopDetails, "Id", "Id", feedback.MedicalShopId);
            ViewData["UserId"] = new SelectList(_context.UserDetails, "Id", "Id", feedback.UserId);
            return View(feedback);
        }

        // GET: Feedbacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var UserId = HttpContext.Session.GetInt32("UserId");
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "User");
            }
            var uDetails = await (from u in _context.UserDetails
                                  where u.Id == UserId
                                  select u).FirstOrDefaultAsync();
            ViewBag.ProfilePhoto = uDetails.ProfilePic;
            ViewBag.editId = uDetails.Id;
            if (id == null || _context.Feedbacks == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.MedicalShop)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Feedbacks == null)
            {
                return Problem("Entity set 'pharma_managementContext.Feedbacks'  is null.");
            }
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedbackExists(int id)
        {
          return (_context.Feedbacks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
