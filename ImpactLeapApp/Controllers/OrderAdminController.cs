using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ImpactLeapApp.Data;
using ImpactLeapApp.Models.OrderModels;
using Microsoft.AspNetCore.Identity;
using ImpactLeapApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace ImpactLeapApp.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class OrderAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderAdminController(ApplicationDbContext context, UserManager<ApplicationUser> UserManager)
        {
            _context = context;
            _userManager = UserManager;
        }

        // GET: OrderAdmin
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.ToListAsync());
        }

        // GET: OrderAdmin/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var tempOrder = await _context.Orders.SingleOrDefaultAsync(o => o.OrderId == id);

            if (tempOrder.OrderNum != null)
            {
                ViewData["OrderNum"] = tempOrder.OrderNum;
            }

            if (id == 0)
            {
                return View("~/Views/Shared/Error.cshtml");
            }

            List<OrderDetailViewModel> orderDetailVMs = new List<OrderDetailViewModel>();

            var tempOrderDetails = (from u in _context.Users
                                join o in _context.Orders on u.Id equals o.UserId
                                join od in _context.OrderDetails on o.OrderId equals od.OrderId
                                join m in _context.Modules on od.ModuleId equals m.ModuleId
                                select new
                                {
                                    OrderId = o.OrderId,
                                    OrderNum = o.OrderNum,
                                    UserId = u.Id,
                                    UserEmail = u.Email,
                                    OrderDetailId = od.OrderDetailId,
                                    OrderedDate = o.OrderedDate,
                                    ModuleId = m.ModuleId,
                                    ModuleIds = o.ModuleIds,
                                    ModuleName = m.ModuleName,
                                    NoteFromUser = o.NoteFromUser,
                                    NoteFromAdmin = o.NoteFromAdmin,
                                    UploadedFileName = o.UploadedFileName,
                                    UploadedFilePath = o.UploadedFilePath,
                                    TotalToPay = o.TotalToPay,
                                    SalesRep = o.SalesRep,
                                    DeliveredDate = o.DeliveredDate,
                                    OrderStatus = o.OrderStatus,
                                    Module = od.Module,
                                    PortfolioId = o.PortfolioId,
                                }).ToList();

            var currentOrderDetails = tempOrderDetails.Where(y => y.OrderId == id)
                                                      .ToList();

            foreach (var orderDetail in currentOrderDetails)
            {
                orderDetailVMs.Add(new OrderDetailViewModel()
                {
                    OrderId = orderDetail.OrderId,
                    OrderNum = orderDetail.OrderNum,
                    UserId = orderDetail.UserId,
                    UserEmail = orderDetail.UserEmail,
                    OrderDetailId = orderDetail.OrderDetailId,
                    OrderedDate = orderDetail.OrderedDate,
                    ModuleId = orderDetail.ModuleId,
                    ModuleIds = orderDetail.ModuleIds,
                    ModuleName = orderDetail.ModuleName,
                    NoteFromUser = orderDetail.NoteFromUser,
                    NoteFromAdmin = orderDetail.NoteFromAdmin,
                    UploadedFileName = orderDetail.UploadedFileName,
                    UploadedFilePath = orderDetail.UploadedFilePath,
                    TotalToPay = orderDetail.TotalToPay,
                    SalesRep = orderDetail.SalesRep,
                    DeliveredDate = orderDetail.DeliveredDate,
                    OrderStatus = orderDetail.OrderStatus,
                    Module = orderDetail.Module,
                    PortfolioId = orderDetail.PortfolioId,
                });
            };

            if (orderDetailVMs == null)
            {
                return NotFound();
            }

            var portfolidId = _context.Orders.SingleOrDefault(o => o.OrderId == id).PortfolioId;
            ViewData["OrderId"] = id;
            ViewBag.Portfolio = _context.Portfolios.SingleOrDefault(p => p.PortfolioId == portfolidId);

            return View(orderDetailVMs);
        }

        // GET: OrderAdmin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrderAdmin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,OrderNum,UserEmail,UserId,SalesRep,OrderedDate,DeliveredDate,OrderStatus,NoteFromUser,NoteFromAdmin,ModuleIds,SelectionDiscount,TotalToPay,PromotionId,IsPromotionCodeApplied,UploadedFileName,InvestmentId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: OrderAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.SingleOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: OrderAdmin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,OrderNum,UserEmail,UserId,SalesRep,OrderedDate,DeliveredDate,OrderStatus,NoteFromUser,NoteFromAdmin,ModuleIds,SelectionDiscount,TotalToPay,PromotionId,IsPromotionCodeApplied,UploadedFileName,InvestmentId")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(order);
                _context.Orders.SingleOrDefault(o => o.OrderId == id).ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();

            }
            return RedirectToAction("Details", new { id = id });
        }

        // GET: OrderAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .SingleOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: OrderAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(m => m.OrderId == id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
