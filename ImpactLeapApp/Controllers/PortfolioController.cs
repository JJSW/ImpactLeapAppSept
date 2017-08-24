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
using Microsoft.AspNetCore.Http;

namespace ImpactLeapApp.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private static string _emailAddress;
        private static Int32 _orderId;
        private readonly int _dollarCent = 100; // $10.00 = 1000

        public PortfolioController(ApplicationDbContext context,
                              UserManager<ApplicationUser> UserManager,
                              SignInManager<ApplicationUser> SignInManager,
                              ILoggerFactory loggerFactory)
        {
            _context = context;
            _userManager = UserManager;
            _signInManager = SignInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        // GET: Portfolio
        public async Task<IActionResult> Index(Int32 id)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            if (_signInManager.IsSignedIn(User))
            {
                _emailAddress = await _userManager.GetEmailAsync(user);
                ViewData["Email"] = _emailAddress;
            }

            _orderId = id;
            ViewData["OrderId"] = _orderId;
            ViewBag.TotalPrice = _context.Orders.SingleOrDefault(o => o.OrderId == id).TotalPrice;
            ViewBag.SelectionDiscount = _context.Orders.SingleOrDefault(o => o.OrderId == id).SelectionDiscount;
            ViewBag.SelectionDiscountMethod = _context.Orders.SingleOrDefault(o => o.OrderId == id).SelectionDiscountMethod;

            var tempTotalToPay = _context.Orders.SingleOrDefault(o => o.OrderId == id).TotalToPay;
            ViewBag.TotalToPay = tempTotalToPay / _dollarCent;

            return View(await _context.Portfolios.ToListAsync());
        }

        [HttpPost]
        public IActionResult NewPortfolio(int portfolioId, int orderId)
        {
            _context.Orders.SingleOrDefault(o => o.OrderId == orderId).PortfolioId = portfolioId;
            _context.SaveChanges();

            return RedirectToAction("NewOrder", "Order");
        }

        private bool FundExists(int id)
        {
            return _context.Portfolios.Any(e => e.PortfolioId == id);
        }
    }
}