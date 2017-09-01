using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ImpactLeapApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ImpactLeapApp.Models;
using ImpactLeapApp.Models.OrderModels;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using ImpactLeapApp.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace ImpactLeapApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IHostingEnvironment _environment;
        private readonly ILogger _logger;
        private static string _emailAddress;
        private static string _selectionDiscount;
        private static string _totalToPay;
        private static string _orderNumber;
        private readonly string _externalCookieScheme;
        private static Int32 _orderId;
        private static int _totalPrice;

        private readonly int _dollarCent = 100; // $10.00 = 1000

        public OrderController(ApplicationDbContext context,
                               UserManager<ApplicationUser> UserManager,
                               SignInManager<ApplicationUser> SignInManager,
                               IHostingEnvironment environment,
                               IOptions<IdentityCookieOptions> identityCookieOptions,
                               ILoggerFactory loggerFactory)
        {
            _context = context;
            _userManager = UserManager;
            _environment = environment;
            _signInManager = SignInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
        }

        public async Task<IActionResult> Index(string message, int id)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            List<TempOrderViewModel> tempOrders = new List<TempOrderViewModel>();
            ViewData["Error"] = message;


            // Set portfolio ID
            if (id != 0)
            {
                ViewData["PortfolioId"] = id;
                TempData["IsPortfolioSet"] = true;
            }

            // Check email if a user signed in
            if (_signInManager.IsSignedIn(User))
            {
                _emailAddress = await _userManager.GetEmailAsync(user);
                ViewData["Email"] = _emailAddress;
            }

            // Set saving for multiple selection
            var savingData = _context.Savings.Where(s => s.IsActive == true);

            List<Saving> saving = new List<Saving>(savingData);
            ViewBag.SavingData = saving;

            foreach (var module in _context.Modules)
            {
                tempOrders.Add(new TempOrderViewModel()
                {
                    Modules = module
                });
            }

            return View(tempOrders);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult NewOrder()
        {
            ViewBag.TotalPrice = _totalPrice;
            ViewBag.SelectionDiscount = _selectionDiscount;
            ViewBag.TotalToPay = _totalToPay;

            ViewData["LoggedinOrTempUserId"] = _context.Orders.SingleOrDefault(o => o.OrderId == _orderId).UserId;
            ViewData["PortfolioId"] = _context.Orders.SingleOrDefault(o => o.OrderId == _orderId).PortfolioId;
            ViewData["OrderId"] = _orderId;
            ViewData["OrderNumber"] = _orderNumber;
            ViewData["Email"] = _emailAddress;

            var OrderDetails = _context.OrderDetails.Where(o => o.OrderId == _orderId).Include(o => o.Module);
            return View(OrderDetails.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> NewOrder(IFormCollection collection,
                                                  string email,
                                                  int selectionDiscountMethod,
                                                  int totalPrice,
                                                  string selectionDiscount,
                                                  string totalToPay,
                                                  int portfolioIdFromModule)
        {
            int parsedSelectionDiscount = 0;
            int parsedTotalToPay = 0;
            object parsedSelectionDiscountMethod = null; 

            _totalPrice = totalPrice;
            _selectionDiscount = selectionDiscount;
            _totalToPay = totalToPay;
            
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            ApplicationUser TempUser;

            ViewBag.TotalPrice = _totalPrice;
            ViewBag.SelectionDiscount = _selectionDiscount;
            ViewBag.TotalToPay = _totalToPay;

            // Save temporary data
            if (_signInManager.IsSignedIn(User))
            {
                TempUser = user;
                _emailAddress = await _userManager.GetEmailAsync(user);
                ViewData["Email"] = _emailAddress;
            }
            else
            {
                var findUser = await _userManager.FindByEmailAsync(email);
                var notRegisteredUser = await _userManager.FindByEmailAsync("tempuser@impactleap.com");

                if (findUser != null)
                {
                    TempUser = findUser;
                }
                else
                {
                    TempUser = notRegisteredUser;

                    var tempUser = new ApplicationUser()
                    {
                        Email = email,
                        NormalizedEmail = email.ToUpper(),
                        UserName = email,
                        NormalizedUserName = email.ToUpper(),
                        UserRole = UserRoleList.Temporary,
                        ModifiedDate = DateTime.Now,
                    };

                    _context.ApplicationUsers.Add(tempUser);
                    await _context.SaveChangesAsync();
                }

                _emailAddress = email;
            }

            ViewData["Email"] = _emailAddress;

            parsedSelectionDiscountMethod = ParseValueToDiscountMethod(selectionDiscountMethod);
            parsedSelectionDiscount = ParseStringToInt(_selectionDiscount);
            parsedTotalToPay = ParseStringToInt(_totalToPay);

            ViewBag.TotalToPay = _totalToPay;

            // Set order number
            var ordersFromCurrentUser = _context.Orders.Where(o => o.UserEmail == _emailAddress);

            if (ordersFromCurrentUser == null || !ordersFromCurrentUser.Any())
            {
                _orderNumber = CreateOrderPattern() + "001";
            }
            else
            {
                var currentOrderNumber = ordersFromCurrentUser.OrderByDescending(o => o.OrderNum).FirstOrDefault().OrderNum;
                var orderPattern = CreateOrderPattern();
                var orderSequence = Convert.ToInt32(currentOrderNumber.Substring(6));
                var nextOrderSequece = orderSequence + 1;
                _orderNumber = orderPattern + nextOrderSequece.ToString("D3");
            }

            // Create a order
            _context.Orders.Add(new Order()
            {
                UserEmail = _emailAddress,
                OrderedDate = DateTime.Now,
                UserId = TempUser.Id,
                TotalPrice = _totalPrice,
                SelectionDiscount = parsedSelectionDiscount,
                SelectionDiscountMethod = (SavingDiscountMethodList)parsedSelectionDiscountMethod,
                TotalToPay = parsedTotalToPay * _dollarCent,
                PromotionId = -1,
                OrderNum = _orderNumber,
                ModifiedDate = DateTime.Now,
            });

            await _context.SaveChangesAsync();

            _orderId = _context.Orders.LastOrDefault(o => o.UserId == TempUser.Id).OrderId;

            // Create order detail
            CreateOrderDetail(collection);

            CreateModuleIds();

            var orderDetails = _context.OrderDetails.Where(od => od.OrderId == _orderId).Include(od => od.Module);

            ViewData["OrderId"] = _orderId;
            ViewData["OrderNumber"] = _orderNumber;

            return View(orderDetails.ToList());

            /* Check portfolio - removed by request
            // Check portfolio
            var portfolioIdByOrder = _context.Orders.SingleOrDefault(od => od.OrderId == _orderId).PortfolioId;

            if (portfolioIdByOrder != 0)
            {
                return View(orderDetails.ToList());
            }
            else
            {
                // Case of portfolio to module
                if (portfolioIdFromModule != 0)
                {
                    _context.Orders.SingleOrDefault(od => od.OrderId == _orderId).PortfolioId = portfolioIdFromModule;
                    _context.SaveChanges();

                    return View(orderDetails.ToList());
                }
                // Case of module to portfolio
                else
                {
                    return RedirectToAction("Index", "Portfolio", new { id = _orderId });
                }
            }
            */
        }

        #region New Order Helpers

        private SavingDiscountMethodList ParseValueToDiscountMethod(int value)
        {
            var result = SavingDiscountMethodList.Fixed;

            if (value == 0)
            {
                result = SavingDiscountMethodList.Fixed;
            }
            else if (value == 1)
            {
                result = SavingDiscountMethodList.Percentage;
            }

            return result;
        }

        private int ParseStringToInt(string str)
        {
            double tempValue = 0.0;
            int parsedResult = 0;

            if (double.TryParse(str, out tempValue))
            {
                parsedResult = (int)tempValue;
            }
            else
            {
                _logger.LogWarning(1, "Fail to parse the string to int.");
                parsedResult = 0;
            }

            return parsedResult;
        }

        private void CreateOrderDetail(IFormCollection collection)
        {
            var lists = collection["modules"];
            foreach (var list in lists)
            {
                var jsonObj = JsonConvert.DeserializeObject<TempOrderViewModel>(list);

                _context.OrderDetails.Add(new OrderDetail()
                {
                    ModifiedDate = DateTime.Now,
                    OrderId = _context.Orders.LastOrDefault(o => o.OrderId == _orderId).OrderId,
                    ModuleId = jsonObj.Modules.ModuleId,
                    ModuleName = jsonObj.Modules.ModuleName
                });
            }

            _context.SaveChanges();
        }

        private string CreateOrderPattern()
        {
            string today = DateTime.Today.ToString("MMdd");
            return "IM" + today;
        }

        private void CreateModuleIds()
        {
            var moduleIdList = _context.OrderDetails.Where(od => od.OrderId == _orderId).Select(s => s.ModuleId);
            var moduleIds = "";
            foreach (var moduleId in moduleIdList)
            {
                moduleIds += moduleId + " ";
            }

            _context.Orders.SingleOrDefault(o => o.OrderId == _orderId).ModuleIds = moduleIds;
            _context.SaveChanges();
        }

        #endregion

        // GET: Orders
        public async Task<IActionResult> Orders()
        {
            var applicationDbContext = _context.Orders;
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, string noteFromUser)
        {
            ViewBag.TotalToPay = _totalToPay;
            string uploadDate = DateTime.Now.ToString("ddMMyyyy");
            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads/" + uploadDate + "/" + _emailAddress);
            string uploadPathLink = HttpContext.Request.Scheme + "://" +
                                    HttpContext.Request.Host +
                                    "/uploads/" + uploadDate + "/" + _emailAddress + "/";

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            if (file.Length > 0)
            {
                using (var fileStream = new FileStream(Path.Combine(uploadPath, file.FileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                _context.Orders.SingleOrDefault(o => o.OrderId == _orderId).UploadedFileName += file.FileName + " ";
            }

            if (noteFromUser != null)
            {
                _context.Orders.SingleOrDefault(o => o.OrderId == _orderId).NoteFromUser = noteFromUser;
            }

            _context.Orders.SingleOrDefault(o => o.OrderId == _orderId).UploadedFilePath = uploadPathLink;

            ViewData["OrderId"] = _orderId;

            await _context.SaveChangesAsync();

            return RedirectToAction("NewOrder");
        }

        [HttpPost]
        public string SubmitNote(string noteFromUser)
        {
            if (noteFromUser != null)
            {
                _context.Orders.SingleOrDefault(o => o.OrderId == _orderId).NoteFromUser = noteFromUser;
            }

            _context.SaveChanges();

            return "Saved";
        }

        [HttpGet]
        public IActionResult GoRegisterPartial()
        {
            RegisterViewModel model = new RegisterViewModel();

            return PartialView("_Register", model);
        }

        [HttpGet]
        public async Task<IActionResult> CheckTempUser(int id)
        {
            ViewData["OrderId"] = _orderId;
            string userEmail = _context.Orders.FirstOrDefault(o => o.OrderId == id).UserEmail;

            if (await _userManager.FindByEmailAsync(userEmail) != null)
            {
                if (_context.ApplicationUsers.SingleOrDefault(u => u.Email == userEmail).PasswordHash == null)
                {
                    ViewData["CheckUser"] = "NeedRegister";
                }
                else
                {
                    ViewData["CheckUser"] = "NeedLogin";
                }
            }

            return View("CheckTempUser");
        }

        //
        // GET: /Account/_Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult PartialRegister(string returnUrl = null)
        {
            ViewData["Email"] = _emailAddress;
            ViewData["OrderId"] = _orderId;
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PartialRegister(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Email"] = _emailAddress;
            ViewData["OrderId"] = _orderId;

            if (ModelState.IsValid)
            {
                if (_emailAddress != null)
                {
                    var tempUser = _context.ApplicationUsers.FirstOrDefault(u => u.Email == _emailAddress);

                    tempUser.UserName = model.Email;
                    tempUser.NormalizedUserName = model.Email.ToUpper();
                    tempUser.Email = model.Email;
                    tempUser.EmailConfirmed = true;
                    tempUser.NormalizedEmail = model.Email.ToUpper();
                    tempUser.CompanyName = model.CompanyName;

                    tempUser.ModifiedDate = DateTime.Now;
                    tempUser.UserRole = UserRoleList.Member;

                    var result = await _userManager.AddPasswordAsync(tempUser, model.Password);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(tempUser, isPersistent: false);

                        _context.Orders.FirstOrDefault(o => o.OrderId == _orderId).UserId = tempUser.Id;
                        await _context.SaveChangesAsync();

                        _logger.LogInformation(3, "User created a new account with password.");
                        await _userManager.AddToRoleAsync(tempUser, "Member");

                        return RedirectToAction("NewOrder");
                    }
                    AddErrors(result);
                }
            }

            return View(model);
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PartialLogin(string returnUrl = null)
        {
            ViewData["Email"] = _emailAddress;
            ViewData["OrderId"] = _orderId;
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.Authentication.SignOutAsync(_externalCookieScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PartialLogin(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Email"] = _emailAddress;
            ViewData["OrderId"] = _orderId;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return RedirectToAction("NewOrder");
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}