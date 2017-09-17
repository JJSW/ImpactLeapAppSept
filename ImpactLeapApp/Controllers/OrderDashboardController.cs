using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ImpactLeapApp.Models;
using ImpactLeapApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ImpactLeapApp.Models.OrderModels;

namespace ImpactLeapApp.Controllers
{
    public class OrderDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderDashboardController(ApplicationDbContext context,
                                      UserManager<ApplicationUser> UserManager)
        {
            _context = context;
            _userManager = UserManager;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            var userId = user.Id;

            return View(await _context.Orders.Where(m => m.UserId == userId).OrderBy(m => m.OrderId).ToListAsync());
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            if (_context.Orders.SingleOrDefault(o => o.OrderId == id).OrderNum != null)
            {
                ViewData["OrderNum"] = _context.Orders.SingleOrDefault(o => o.OrderId == id).OrderNum;
            }

            if (id == 0)
            {
                return View("~/Views/Shared/Error.cshtml");
            }

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            List<OrderDetailViewModel> orderDetailVMs = new List<OrderDetailViewModel>();
            var userId = user.Id;

            var orderDetails = (from u in _context.Users
                                join o in _context.Orders on u.Id equals o.UserId
                                join od in _context.OrderDetails on o.OrderId equals od.OrderId
                                join m in _context.Modules on od.ModuleId equals m.ModuleId
                                select new
                                {
                                    OrderId = o.OrderId,
                                    OrderNum = o.OrderNum,
                                    UserId = u.Id,
                                    OrderDetailId = od.OrderDetailId,
                                    ModuleId = m.ModuleId,
                                    ModuleName = m.ModuleName,
                                    NoteFromUser = o.NoteFromUser,
                                    NoteFromAdmin = o.NoteFromAdmin,
                                    UploadedFileName = o.UploadedFileName,
                                    UploadedFilePath = o.UploadedFilePath,
                                    TotalToPay = o.TotalToPay,
                                    PortfolioId = o.PortfolioId,
                                }).ToList();

            var currentOrderDetails = orderDetails.Where(x => x.UserId == userId)
                                                  .Where(y => y.OrderId == id)
                                                  .ToList();

            foreach (var orderDetail in currentOrderDetails)
            {
                orderDetailVMs.Add(new OrderDetailViewModel()
                {
                    OrderId = orderDetail.OrderId,
                    OrderNum = orderDetail.OrderNum,
                    UserId = orderDetail.UserId,
                    OrderDetailId = orderDetail.OrderDetailId,
                    ModuleId = orderDetail.ModuleId,
                    ModuleName = orderDetail.ModuleName,
                    NoteFromUser = orderDetail.NoteFromUser,
                    NoteFromAdmin = orderDetail.NoteFromAdmin,
                    UploadedFileName = orderDetail.UploadedFileName,
                    UploadedFilePath = orderDetail.UploadedFilePath,
                    TotalToPay = orderDetail.TotalToPay,
                    PortfolioId = orderDetail.PortfolioId,
                });
            };

            if (orderDetailVMs == null)
            {
                return NotFound();
            }

            // Pass uploaded file list
            ViewData["OrderId"] = id;
            ViewBag.UploadedFileList = new string[] { "" };

            if (_context.Orders.SingleOrDefault(o => o.OrderId == id).UploadedFileName != null)
            {
                var tempUploadedFile = _context.Orders.SingleOrDefault(o => o.OrderId == id).UploadedFileName;

                if (tempUploadedFile != null)
                    ViewBag.UploadedFileList = tempUploadedFile.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            var portfolidId = _context.Orders.SingleOrDefault(o => o.OrderId == id).PortfolioId;
            ViewBag.Portfolio = _context.Portfolios.SingleOrDefault(p => p.PortfolioId == portfolidId);

            return View(orderDetailVMs);
        }

        // GET
        public async Task<IActionResult> EditNote(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.SingleOrDefaultAsync(m => m.OrderNum == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNote(string OrderNum, string NoteFromUser)
        {

            if (ModelState.IsValid)
            {
                _context.Orders.SingleOrDefault(o => o.OrderNum == OrderNum).NoteFromUser = NoteFromUser;
                _context.Orders.SingleOrDefault(o => o.OrderNum == OrderNum).ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            var id = _context.Orders.SingleOrDefault(o => o.OrderNum == OrderNum).OrderId;
            return RedirectToAction("OrderDetails", new { id = id });
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}