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
    public class AgencyMedicalOrdersController : Controller
    {
        private readonly pharma_managementContext _context;

        public AgencyMedicalOrdersController(pharma_managementContext context)
        {
            _context = context;
        }

        // GET: AgencyMedicalOrders
        public async Task<IActionResult> Index()
        {
            var pharma_managementContext = _context.MedicalOrders.Include(m => m.Company).Include(m => m.MedicalShop).Include(m => m.Product);
            return View(await pharma_managementContext.ToListAsync());
        }

        [HttpPost]
        public IActionResult AcceptOrder(int verify,int pid,int oid,int mid,int quantity,MedicalOrder medicalOrder,MedicalShopProductStock medicalShopStock,AgencyProductStock agencyProductStock)
        {
            var stockDetail = (from a in _context.AgencyProductStocks
                    where a.ProductId == pid
                    select a).FirstOrDefault();
            if(stockDetail.TotalQuantity < quantity)
            {
                TempData["error"] = "Not enough product quantity";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var orderDeails = (from o in _context.MedicalOrders
                             where o.Id == oid
                             select o).FirstOrDefault();

                orderDeails.IsPlaced = verify;

                stockDetail.TotalQuantity = stockDetail.TotalQuantity - quantity;
       
                _context.SaveChanges();
                
                var sDetail = (from p in _context.MedicalShopProductStocks
                             where p.ProductId == pid
                             select p).FirstOrDefault();

                if(sDetail != null)
                {
                    sDetail.TotalQuantity += quantity;
                }
                else
                {
                    medicalShopStock.MedicalShopId = mid;
                    medicalShopStock.ProductId = pid;
                    medicalShopStock.TotalQuantity = quantity;
                    _context.MedicalShopProductStocks.Add(medicalShopStock);
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: AgencyMedicalOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MedicalOrders == null)
            {
                return NotFound();
            }

            var medicalOrder = await _context.MedicalOrders
                .Include(m => m.Company)
                .Include(m => m.MedicalShop)
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalOrder == null)
            {
                return NotFound();
            }

            return View(medicalOrder);
        }

        // GET: AgencyMedicalOrders/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id");
            ViewData["MedicalShopId"] = new SelectList(_context.MedicalShopDetails, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.ProductDetails, "Id", "Id");
            return View();
        }

        // POST: AgencyMedicalOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MedicalShopId,ProductId,CompanyId,TotalQuantity,BillAmount,OrderDate,IsPlaced")] MedicalOrder medicalOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicalOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id", medicalOrder.CompanyId);
            ViewData["MedicalShopId"] = new SelectList(_context.MedicalShopDetails, "Id", "Id", medicalOrder.MedicalShopId);
            ViewData["ProductId"] = new SelectList(_context.ProductDetails, "Id", "Id", medicalOrder.ProductId);
            return View(medicalOrder);
        }

        // GET: AgencyMedicalOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MedicalOrders == null)
            {
                return NotFound();
            }

            var medicalOrder = await _context.MedicalOrders.FindAsync(id);
            if (medicalOrder == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id", medicalOrder.CompanyId);
            ViewData["MedicalShopId"] = new SelectList(_context.MedicalShopDetails, "Id", "Id", medicalOrder.MedicalShopId);
            ViewData["ProductId"] = new SelectList(_context.ProductDetails, "Id", "Id", medicalOrder.ProductId);
            return View(medicalOrder);
        }

        // POST: AgencyMedicalOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MedicalShopId,ProductId,CompanyId,TotalQuantity,BillAmount,OrderDate,IsPlaced")] MedicalOrder medicalOrder)
        {
            if (id != medicalOrder.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalOrderExists(medicalOrder.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Manufacturers, "Id", "Id", medicalOrder.CompanyId);
            ViewData["MedicalShopId"] = new SelectList(_context.MedicalShopDetails, "Id", "Id", medicalOrder.MedicalShopId);
            ViewData["ProductId"] = new SelectList(_context.ProductDetails, "Id", "Id", medicalOrder.ProductId);
            return View(medicalOrder);
        }

        // GET: AgencyMedicalOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MedicalOrders == null)
            {
                return NotFound();
            }

            var medicalOrder = await _context.MedicalOrders
                .Include(m => m.Company)
                .Include(m => m.MedicalShop)
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalOrder == null)
            {
                return NotFound();
            }

            return View(medicalOrder);
        }

        // POST: AgencyMedicalOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MedicalOrders == null)
            {
                return Problem("Entity set 'pharma_managementContext.MedicalOrders'  is null.");
            }
            var medicalOrder = await _context.MedicalOrders.FindAsync(id);
            if (medicalOrder != null)
            {
                _context.MedicalOrders.Remove(medicalOrder);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalOrderExists(int id)
        {
          return (_context.MedicalOrders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
