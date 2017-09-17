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
using ImpactLeapApp.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using ImpactLeapApp.Models.ManageViewModels;
using Microsoft.Extensions.Logging;
using static ImpactLeapApp.Controllers.ManageController;

namespace ImpactLeapApp.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class ApplicationUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private static string _userId;
        private static string _message;

        public ApplicationUserController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<ManageController>();
        }

        // GET: ApplicationUser
        public async Task<IActionResult> Index()
        {
            _message = "";
            var orderedUsers = await _context.ApplicationUsers
                                .Where(u => u.Email != "tempuser@impactleap.com")
                                .OrderBy(u => u.UserRole)
                                .ThenBy(u => u.ModifiedDate)
                                .ToListAsync();

            return View(orderedUsers);
        }

        // GET: ApplicationUser/Details/5
        public async Task<IActionResult> Details(string id)
        {
            _message = "";
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // GET: ApplicationUser/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ApplicationUser/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,
                    CompanyName = model.CompanyName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserRole = model.UserRole,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var createdUser = _context.ApplicationUsers.SingleOrDefault(u => u.Email == model.Email);
                    createdUser.NormalizedEmail = createdUser.Email.ToUpper();
                    createdUser.NormalizedUserName = createdUser.UserName.ToUpper();
                    createdUser.ModifiedDate = DateTime.Now;

                    await _userManager.AddToRoleAsync(user, model.UserRole.ToString());
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }

                AddErrors(result);
            }
            return View(model);
        }

        // GET: ApplicationUser/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            ViewData["Message"] = _message;
            return View(applicationUser);
        }

        // POST: ApplicationUser/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("FirstName,LastName,CompanyName,NewsletterRequired,UserRole,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(applicationUser);
                _context.ApplicationUsers.SingleOrDefault(u => u.Id == id).ModifiedDate = DateTime.Now;
                
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = id });
        }

        // GET: ApplicationUser/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // POST: ApplicationUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationUser = await _context.ApplicationUsers.SingleOrDefaultAsync(m => m.Id == id);
            _context.ApplicationUsers.Remove(applicationUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.ApplicationUsers.Any(e => e.Id == id);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }


        //
        // GET: /ApplicationUser/ResetPasswordAdmin
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordAdmin(string id)
        {
            _userId = id;
            return View();
        }

        //
        // POST: /ApplicationUser/ResetPasswordAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordAdmin(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByIdAsync(_userId); 
            if (user != null)
            {
                await _userManager.RemovePasswordAsync(user);
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation(3, "User changed their password successfully.");
                    _message = "Password changed";
                    return RedirectToAction("Edit", new { @id = _userId });
                }
                AddErrors(result);
            }
            return View(model);
        }

 
    }
}
