using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using ImpactLeapApp.Models.BillingModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Principal;
using ImpactLeapApp.Common;
using ImpactLeapApp.Models;
using Microsoft.AspNetCore.Authorization;
using ImpactLeapApp.Data;
using ImpactLeapApp.Models.OrderModels;
using Microsoft.EntityFrameworkCore;

namespace ImpactLeapApp.Controllers
{
    [Authorize]
    public class BillingController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private static int _amountInt;
        private static string _emailAddress;
        private static string _promotionDiscountRate;
        private static string _totalToPay;
        private static Int32 _promotionId;
        private static Int32 _orderId;
        private static PromotionStatusList _promotionStatus;


        // To be able to sent total amount to Stripe API, makes cent digits to 100
        private int _dollarCent = 100;

        public BillingController(ApplicationDbContext context,
                                 UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// Return billing page with a data that contains information with order and module.
        /// Only displays the specific order of the logged in user.
        /// Billing address will be displayed in any circumstances.
        [HttpGet]
        public async Task<IActionResult> Index(int? id)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            List<BillingDetailViewModel> billingVMs = new List<BillingDetailViewModel>();
            var totalToPay = 0;
            var moduleCount = 0;

            _promotionDiscountRate = "0";

            if (_signInManager.IsSignedIn(User))
            {
                _emailAddress = await _userManager.GetEmailAsync(user);
                ViewData["Email"] = _emailAddress;
            }

            if (id != null && id != 0)
            {
                var billingDetails = (from u in _context.Users
                                      join o in _context.Orders on u.Id equals o.UserId
                                      join od in _context.OrderDetails on o.OrderId equals od.OrderId
                                      join m in _context.Modules on od.ModuleId equals m.ModuleId
                                      select new
                                      {
                                          OrderId = o.OrderId,
                                          OrderNum = o.OrderNum,
                                          OrderedDate = o.OrderedDate,
                                          UserId = u.Id,
                                          ModuleId = m.ModuleId,
                                          ModuleName = m.ModuleName,
                                          UnitPrice = m.UnitPrice,
                                          TotalToPay = o.TotalToPay,
                                          OrderStatus = o.OrderStatus,
                                          NoteFromUser = o.NoteFromUser,
                                          UploadedFileName = o.UploadedFileName,
                                          PortfolidId = o.PortfolioId,
                                      }).ToList();

                var temps = billingDetails.Where(x => x.UserId == user.Id).Where(y => y.OrderId == id).ToList();

                foreach (var billing in temps)
                {
                    billingVMs.Add(new BillingDetailViewModel()
                    {
                        OrderId = billing.OrderId,
                        OrderNum = billing.OrderNum,
                        OrderedDate = billing.OrderedDate,
                        UserId = billing.UserId,
                        ModuleId = billing.ModuleId,
                        ModuleName = billing.ModuleName,
                        UnitPrice = billing.UnitPrice,
                        TotalToPay = billing.TotalToPay,
                        OrderStatus = billing.OrderStatus,
                        NoteFromUser = billing.NoteFromUser,
                        UploadedFileName = billing.UploadedFileName,
                        PortfolioId = billing.PortfolidId,
                    });
                };

                foreach (var billing in billingVMs)
                {
                    moduleCount += 1;
                    totalToPay = billing.TotalToPay;
                }

                ViewBag.BillingDetails = billingVMs;
                ViewBag.SavingDiscount = _context.Orders.SingleOrDefault(o => o.OrderId == id).SavingDiscount;
                ViewBag.SavingDiscountMethod = _context.Orders.SingleOrDefault(o => o.OrderId == id).SavingDiscountMethod;
                ViewBag.TotalPrice = _context.Orders.SingleOrDefault(o => o.OrderId == id).TotalPrice;
                var tempTotalToPay = _context.Orders.SingleOrDefault(o => o.OrderId == id).TotalToPay;
                ViewBag.TotalToPay = tempTotalToPay / _dollarCent;

                ViewData["TotalToPay"] = totalToPay;
                ViewData["TotalToPayDisplay"] = totalToPay / _dollarCent;
                ViewData["ModuleCount"] = moduleCount;
                ViewData["OrderId"] = id;
                _orderId = (int)id;
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var billingAddress = await _context.BillingAddresses.LastOrDefaultAsync(x => x.UserId == userId);
            _amountInt = totalToPay;
          
            ViewBag.PromotionStatus = PromotionStatusList.Ready;
            TempData["PromotionDiscountRate"] = _promotionDiscountRate;

            ViewData["BillingAddressId"] = (billingAddress != null) ? (int)billingAddress.BillingAddressId : -1;
            ViewBag.BillingAddress = billingAddress;

            return View(billingVMs);
        }

        /// Get order when a user select the default module.
        /// Since the default module is free of charge, no need to go through payment process.
        public async Task<IActionResult> ChargeDefault(int id)
        {
            var completedOrders = _context.Orders.Where(x => x.OrderId == id);

            foreach (var order in completedOrders)
            {
                order.OrderStatus = OrderStatusList.Processing;
            }

            await _context.SaveChangesAsync();

            return View(completedOrders);
        }

        /// Execute Stripe API.  
        /// Create customer and charge tokens for request.
        /// After successful payment, the order's status set to completed.
        public async Task<IActionResult> Charge(string stripeToken,
                                                string stripeEmail,
                                                int orderId,
                                                int bAddressId)
        {
            var customers = new StripeCustomerService();
            var charges = new StripeChargeService();

            var completedOrders = _context.Orders.Where(x => x.OrderId == orderId);

            var customer = customers.Create(new StripeCustomerCreateOptions
            {
                Email = stripeEmail,
                SourceToken = stripeToken,
            });

            var charge = charges.Create(new StripeChargeCreateOptions
            {
                Amount = _amountInt,
                Description = "Module Charge",
                Currency = "USD",
                CustomerId = customer.Id,
            });

            foreach (var order in completedOrders)
            {
                order.OrderStatus = OrderStatusList.Processing;
                order.ModifiedDate = DateTime.Now;
            }

            _context.Orders.SingleOrDefault(o => o.OrderId == orderId).IsPromotionCodeApplied = true;

            await _context.SaveChangesAsync();

            return View(completedOrders);
        }

        public async Task<IActionResult> Cancel(int id)
        {
            var orders = _context.Orders.Where(x => x.OrderId == id);

            foreach (var order in orders)
            {
                order.OrderStatus = OrderStatusList.Cancelled;
                order.ModifiedDate = DateTime.Now;
                order.PromotionId = -1;
            }

            await _context.SaveChangesAsync();

            return View(orders);
        }


        [HttpGet]
        public IActionResult SubmitPromoCode()
        {
            Promotion promotion = new Promotion();
            return PartialView(promotion);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPromoCode(Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                var verfiedPromotion = _context.Promotions.FirstOrDefault(p => p.PromotionCode.Equals(promotion.PromotionCode));

                if (verfiedPromotion != null)
                {
                    var verfiedPromotionId = verfiedPromotion.PromotionId;
                    bool isPromotionCodeAppliedToOrder = _context.Orders.SingleOrDefault(s => s.OrderId == _orderId).IsPromotionCodeApplied;

                    var userId = _userManager.GetUserId(HttpContext.User);
                    var allOrdersFromCurrentUser = _context.Orders.Where(o => o.UserId == userId);
                    bool isPromotionCodeAppliedToUser = allOrdersFromCurrentUser.Select(a => a.PromotionId).Any(a => a.Equals(verfiedPromotionId));

                    if (verfiedPromotion.DateFrom <= DateTime.Now
                        && verfiedPromotion.DateTo >= DateTime.Now
                        && verfiedPromotion.IsActive)
                    {
                        // Check if there is already a promotion code is applied
                        if (isPromotionCodeAppliedToOrder == false
                            && isPromotionCodeAppliedToUser == false)
                        {
                            var tempTotalAmount = _context.Orders.SingleOrDefault(o => o.OrderId == _orderId).TotalToPay;

                            if (verfiedPromotion.DiscountMethod == PromotionDiscountMethodList.Fixed)
                            {
                                var promotionDiscountRate = verfiedPromotion.DiscountRate;
                                tempTotalAmount = (tempTotalAmount - (promotionDiscountRate * _dollarCent));
                                _promotionDiscountRate = "-$" + promotionDiscountRate;
                                TempData["PromotionDiscountRate"] = _promotionDiscountRate;
                            }
                            else if (verfiedPromotion.DiscountMethod == PromotionDiscountMethodList.Percentage)
                            {
                                var tempTotalAmountWithCent = tempTotalAmount / _dollarCent;
                                var promotionDiscountRate = verfiedPromotion.DiscountRate;
                                var promotionDicountRatePercent = promotionDiscountRate * 0.01;
                                var discountRateFixed = tempTotalAmountWithCent * promotionDicountRatePercent;

                                tempTotalAmount = (int)((tempTotalAmountWithCent - discountRateFixed) * _dollarCent);
                                _promotionDiscountRate = "-$" + discountRateFixed + " (" + promotionDiscountRate + "%" + ")";

                                TempData["PromotionDiscountRate"] = _promotionDiscountRate;
                            }

                            _context.Orders.SingleOrDefault(o => o.OrderId == _orderId).TotalToPay = tempTotalAmount;
                            _context.Orders.SingleOrDefault(o => o.OrderId == _orderId).PromotionId = verfiedPromotionId;
                            _promotionId = verfiedPromotionId;

                            await _context.SaveChangesAsync();

                            _totalToPay = (tempTotalAmount / _dollarCent).ToString();

                            ViewBag.PromotionStatus = PromotionStatusList.Applied;
                            _promotionStatus = PromotionStatusList.Applied;
                        }
                        else
                        {
                            ViewBag.PromotionStatus = PromotionStatusList.Used;
                            _promotionStatus = PromotionStatusList.Used;
                        }
                    }
                }
                else
                {
                    return Json(new { success = false });
                }
            }

            return View(promotion);
        }


        public IActionResult Error()
        {
            return View();
        }

        /// Displays billing address for the logged in user.
        public async Task<IActionResult> BillingAddress()
        {
            var userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            var billingAddress = await _context.BillingAddresses.LastOrDefaultAsync(w => w.UserId == userId);

            return View(billingAddress);
        }

        /// Get billing address for the logged in user.
        /// A user is able to have only one billing address, 
        /// so any updates will overwrite the previous address.
        public async Task<IActionResult> BillingAddress(BillingAddress billingAddress)
        {
            if (ModelState.IsValid)
            {
                var userId = User.GetUserId();
                var user = await _userManager.FindByIdAsync(userId);
                var currentBillingAddresses = _context.BillingAddresses
                    .Where(x => x.UserId == userId).ToList();

                if (currentBillingAddresses.Any())
                {
                    foreach (var address in currentBillingAddresses)
                    {
                        _context.BillingAddresses.Remove(address);
                    }

                    await _context.SaveChangesAsync();
                }

                billingAddress.UserId = userId;
                user.BillingAddress = billingAddress;
                await _userManager.UpdateAsync(user);

                return RedirectToAction("Index");
            }

            return View(billingAddress);
        }

    }
}