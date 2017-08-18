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
    public class FundController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FundController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Fund
        public async Task<IActionResult> Index()
        {
            return View(await _context.Funds.ToListAsync());
        }


        private bool FundExists(int id)
        {
            return _context.Funds.Any(e => e.FundId == id);
        }
    }
}
