using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImpactLeapApp.Data;
using ImpactLeapApp.Models.OrderModels;
using Microsoft.AspNetCore.Authorization;

namespace ImpactLeapApp.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class SavingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private int _minSelectFrom = 0;
        private int _maxSelectFrom = 0;
        private int _minSelectTo = 0;
        private int _maxSelectTo = 0;

        public SavingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Saving
        public async Task<IActionResult> Index()
        {
            /* For displaying range
            Dictionary<int, List<int>> savingData = new Dictionary<int, List<int>>();            
            foreach(var savingID in _context.Savings.Select(s=>s.SavingId))
            {
                var tempSelectFrom = _context.Savings.SingleOrDefault(s => s.SavingId == savingID).SelectFrom;
                var tempSelectTo = _context.Savings.SingleOrDefault(s => s.SavingId == savingID).SelectTo;
                savingData[savingID] = new List<int> { tempSelectFrom, tempSelectTo };
            }

            ViewBag.SavingData = savingData;
            ViewBag.MinimumSelectedFrom = _context.Savings.OrderBy(s => s.SelectFrom).First().SelectFrom;
            ViewBag.MaximumSelctedTo = _context.Savings.OrderByDescending(s => s.SelectTo).First().SelectTo;
            */

            return View(await _context.Savings.ToListAsync());
        }

        // GET: Saving/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saving = await _context.Savings
                .SingleOrDefaultAsync(m => m.SavingId == id);
            if (saving == null)
            {
                return NotFound();
            }

            return View(saving);
        }

        // GET: Saving/Create
        public IActionResult Create()
        {
            if (_context.Savings.Any())
            {
                if (_context.Savings.Any())
                {
                    _minSelectFrom = _context.Savings.OrderBy(s => s.SelectFrom).First().SelectFrom;
                    _maxSelectFrom = _context.Savings.OrderByDescending(s => s.SelectFrom).First().SelectFrom;
                    _minSelectTo = _context.Savings.OrderBy(s => s.SelectTo).First().SelectTo;
                    _maxSelectTo = _context.Savings.OrderByDescending(s => s.SelectTo).First().SelectTo;
                }
                else
                {
                    _minSelectFrom = 0;
                    _maxSelectFrom = 0;
                    _minSelectTo = 0;
                    _maxSelectTo = 0;
                }

                ViewData["MinSelectFrom"] = _minSelectFrom;
                ViewData["MaxSelectFrom"] = _maxSelectFrom;
                ViewData["MinSelectTo"] = _minSelectTo;
                ViewData["MaxSelectTo"] = _maxSelectTo;
            }
            return View();
        }

        // POST: Saving/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SavingId,SavingName,DiscountMethod,SavingRate,SelectFrom,SelectTo,Description,IsActive")] Saving saving)
        {
            if (_context.Savings.Any())
            {
                _minSelectFrom = _context.Savings.OrderBy(s => s.SelectFrom).First().SelectFrom;
                _maxSelectFrom = _context.Savings.OrderByDescending(s => s.SelectFrom).First().SelectFrom;
                _minSelectTo = _context.Savings.OrderBy(s => s.SelectTo).First().SelectTo;
                _maxSelectTo = _context.Savings.OrderByDescending(s => s.SelectTo).First().SelectTo;
            }

            bool isValidRange = false;

            if (ModelState.IsValid)
            {
                if (saving.SelectFrom < _minSelectFrom)
                {
                    if (saving.SelectTo < _minSelectFrom)
                    {
                        isValidRange = true;
                    }
                }
                else if (saving.SelectFrom > _minSelectFrom)
                {
                    if (saving.SelectFrom > _maxSelectTo)
                    {
                        isValidRange = true;
                    }

                    if (saving.SelectTo < _minSelectTo)
                    {
                        isValidRange = true;
                    }
                }

                if (saving.SelectFrom < _maxSelectFrom)
                {
                    if (saving.SelectFrom > _minSelectTo)
                    {
                        if (saving.SelectTo < _maxSelectTo)
                        {
                            if (saving.SelectTo < _maxSelectFrom)
                            {
                                isValidRange = true;
                            }
                        }
                    }
                }

                if (saving.SelectFrom > saving.SelectTo)
                {
                    ModelState.AddModelError("SelectTo", "Range error : 'select from' must be smaller than 'select to'");
                    return View(saving);
                }

                if (isValidRange)
                {
                    _context.Add(saving);
                    await _context.SaveChangesAsync();
                    var tempSavingId = saving.SavingId;

                    _context.Savings.SingleOrDefault(p => p.SavingId == tempSavingId).ModifiedDate = DateTime.Now;
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError("SelectTo", "Range error");
            return View(saving);
        }

        // GET: Saving/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var currentSelectFrom = _context.Savings.SingleOrDefault(s => s.SavingId == id).SelectFrom;
            var currentSelectTo = _context.Savings.SingleOrDefault(s => s.SavingId == id).SelectTo;

            if (_context.Savings.Count() > 1)
            {
                _minSelectFrom = _context.Savings.Where(s => s.SelectFrom != currentSelectFrom).OrderBy(s => s.SelectFrom).First().SelectFrom;
                _maxSelectFrom = _context.Savings.Where(s => s.SelectFrom != currentSelectFrom).OrderByDescending(s => s.SelectFrom).First().SelectFrom;
                _minSelectTo = _context.Savings.Where(s => s.SelectTo != currentSelectTo).OrderBy(s => s.SelectTo).First().SelectTo;
                _maxSelectTo = _context.Savings.Where(s => s.SelectTo != currentSelectTo).OrderByDescending(s => s.SelectTo).First().SelectTo;
            }
            else
            {
                _minSelectFrom = 0;
                _maxSelectFrom = 0;
                _minSelectTo = 0;
                _maxSelectTo = 0;
            }

            ViewData["MinSelectFrom"] = _minSelectFrom;
            ViewData["MaxSelectFrom"] = _maxSelectFrom;
            ViewData["MinSelectTo"] = _minSelectTo;
            ViewData["MaxSelectTo"] = _maxSelectTo;

            if (id == null)
            {
                return NotFound();
            }

            var saving = await _context.Savings.SingleOrDefaultAsync(m => m.SavingId == id);
            if (saving == null)
            {
                return NotFound();
            }
            return View(saving);
        }

        // POST: Saving/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SavingId,SavingName,DiscountMethod,SavingRate,SelectFrom,SelectTo,Description,IsActive")] Saving saving)
        {
            var currentSelectFrom = _context.Savings.SingleOrDefault(s => s.SavingId == id).SelectFrom;
            var currentSelectTo = _context.Savings.SingleOrDefault(s => s.SavingId == id).SelectTo;

            if (_context.Savings.Count() > 1)
            {
                _minSelectFrom = _context.Savings.Where(s => s.SelectFrom != currentSelectFrom).OrderBy(s => s.SelectFrom).First().SelectFrom;
                _maxSelectFrom = _context.Savings.Where(s => s.SelectFrom != currentSelectFrom).OrderByDescending(s => s.SelectFrom).First().SelectFrom;
                _minSelectTo = _context.Savings.Where(s => s.SelectTo != currentSelectTo).OrderBy(s => s.SelectTo).First().SelectTo;
                _maxSelectTo = _context.Savings.Where(s => s.SelectTo != currentSelectTo).OrderByDescending(s => s.SelectTo).First().SelectTo;
            }
            else
            {
                _minSelectFrom = 0;
                _maxSelectFrom = 0;
                _minSelectTo = 0;
                _maxSelectTo = 0;
            }

            bool isValidRange = false;

            if (id != saving.SavingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (saving.SelectFrom < _minSelectFrom)
                    {
                        if (saving.SelectTo < _minSelectFrom)
                        {
                            isValidRange = true;
                        }
                    }
                    else if (saving.SelectFrom > _minSelectFrom)
                    {
                        if (saving.SelectFrom > _maxSelectTo)
                        {
                            isValidRange = true;
                        }

                        if (saving.SelectTo < _minSelectTo)
                        {
                            isValidRange = true;
                        }
                    }

                    if (saving.SelectFrom < _maxSelectFrom)
                    {
                        if (saving.SelectFrom > _minSelectTo)
                        {
                            if (saving.SelectTo < _maxSelectTo)
                            {
                                if (saving.SelectTo < _maxSelectFrom)
                                {
                                    isValidRange = true;
                                }
                            }
                        }
                    }

                    foreach(var selectFrom in _context.Savings.Select(s => saving.SelectFrom))
                    {
                        foreach(var selectTo in _context.Savings.Select(s => s.SelectTo))
                        {
                            if(selectFrom == selectTo)
                            {
                                ModelState.AddModelError("SelectTo", "Range error : 'select from' can't be overlapped 'select to'");
                                return View(saving);
                            }
                        }
                    }

                    if (saving.SelectFrom > saving.SelectTo)
                    {
                        ModelState.AddModelError("SelectTo", "Range error : 'select from' must be smaller than 'select to'");
                        return View(saving);
                    }

                    if (isValidRange)
                    {
                        _context.Savings.SingleOrDefault(p => p.SavingId == id).SavingName = saving.SavingName;
                        _context.Savings.SingleOrDefault(p => p.SavingId == id).DiscountMethod = saving.DiscountMethod;
                        _context.Savings.SingleOrDefault(p => p.SavingId == id).SavingRate = saving.SavingRate;
                        _context.Savings.SingleOrDefault(p => p.SavingId == id).SelectFrom = saving.SelectFrom;
                        _context.Savings.SingleOrDefault(p => p.SavingId == id).SelectTo = saving.SelectTo;
                        _context.Savings.SingleOrDefault(p => p.SavingId == id).Description = saving.Description;
                        _context.Savings.SingleOrDefault(p => p.SavingId == id).IsActive = saving.IsActive;
                        _context.Savings.SingleOrDefault(p => p.SavingId == id).ModifiedDate = DateTime.Now;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SavingExists(saving.SavingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ModelState.AddModelError("SelectTo", "Range error");
            return View(saving);
        }

        // GET: Saving/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saving = await _context.Savings
                .SingleOrDefaultAsync(m => m.SavingId == id);
            if (saving == null)
            {
                return NotFound();
            }

            return View(saving);
        }

        // POST: Saving/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var saving = await _context.Savings.SingleOrDefaultAsync(m => m.SavingId == id);
            _context.Savings.Remove(saving);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool SavingExists(int id)
        {
            return _context.Savings.Any(e => e.SavingId == id);
        }
    }
}
