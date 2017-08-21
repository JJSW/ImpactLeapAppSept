using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ImpactLeapApp.Data;
using ImpactLeapApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace ImpactLeapApp.Controllers
{
    public class FundController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private static string _emailAddress;

        public FundController(ApplicationDbContext context,
                              UserManager<ApplicationUser> UserManager,
                              SignInManager<ApplicationUser> SignInManager,
                              ILoggerFactory loggerFactory)
        {
            _context = context;
            _userManager = UserManager;
            _signInManager = SignInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        // GET: Fund
        public async Task<IActionResult> Index(string message)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["Error"] = message;

            if (_signInManager.IsSignedIn(User))
            {
                _emailAddress = await _userManager.GetEmailAsync(user);
                ViewData["Email"] = _emailAddress;
            }

            return View(await _context.Funds.ToListAsync());
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult NewFund()
        {
            ViewData["Email"] = _emailAddress;
            return View();
        }


        private bool FundExists(int id)
        {
            return _context.Funds.Any(e => e.FundId == id);
        }
    }
}
