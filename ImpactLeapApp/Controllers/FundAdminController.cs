using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ImpactLeapApp.Data;
using ImpactLeapApp.Models;

namespace ImpactLeapApp.Controllers
{
    public class FundAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FundAdminController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Fund
        public async Task<IActionResult> Index()
        {
            return View(await _context.Funds.ToListAsync());
        }

        // GET: Fund/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fund = await _context.Funds
                .SingleOrDefaultAsync(m => m.FundId == id);
            if (fund == null)
            {
                return NotFound();
            }

            return View(fund);
        }

        // GET: Fund/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fund/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FundId,FundName,FundManager,Strategy,Geography,Description,ModifiedDate")] Fund fund)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fund);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(fund);
        }

        // GET: Fund/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fund = await _context.Funds.SingleOrDefaultAsync(m => m.FundId == id);
            if (fund == null)
            {
                return NotFound();
            }
            return View(fund);
        }

        // POST: Fund/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FundId,FundName,FundManager,Strategy,Geography,Description,ModifiedDate")] Fund fund)
        {
            if (id != fund.FundId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fund);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FundExists(fund.FundId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(fund);
        }

        // GET: Fund/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fund = await _context.Funds
                .SingleOrDefaultAsync(m => m.FundId == id);
            if (fund == null)
            {
                return NotFound();
            }

            return View(fund);
        }

        // POST: Fund/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fund = await _context.Funds.SingleOrDefaultAsync(m => m.FundId == id);
            _context.Funds.Remove(fund);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool FundExists(int id)
        {
            return _context.Funds.Any(e => e.FundId == id);
        }
    }
}
