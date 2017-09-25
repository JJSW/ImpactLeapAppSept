using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ImpactLeapApp.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ImpactLeapApp.Models
{
    public class RoleSeedData
    {
        public static async void Initialize(IServiceProvider isp)
        {
            var context = isp.GetService<ApplicationDbContext>();
            var roleStore = new RoleStore<IdentityRole>(context);

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                await roleStore.CreateAsync(new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                });
            }

            if (!context.Roles.Any(r => r.Name == "Manager"))
            {
                await roleStore.CreateAsync(new IdentityRole
                {
                    Name = "Manager",
                    NormalizedName = "MANAGER"
                });
            }

            if (!context.Roles.Any(r => r.Name == "Member"))
            {
                await roleStore.CreateAsync(new IdentityRole
                {
                    Name = "Member",
                    NormalizedName = "MEMBER"
                });
            }

            if (!context.Roles.Any(r => r.Name == "Unregistered"))
            {
                await roleStore.CreateAsync(new IdentityRole
                {
                    Name = "Unregistered",
                    NormalizedName = "UNREGISTERED"
                });
            }

            var admin = new ApplicationUser
            {
                FirstName = "admin",
                LastName = "administrator",
                Email = "admin@impactleap.com",
                NormalizedEmail = "ADMIN@IMPACTLEAP.COM",
                UserName = "admin@impactleap.com",
                NormalizedUserName = "ADMIN@IMPACTLEAP.COM",
                PhoneNumber = "000-000-0000",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                CompanyName = "Impact Leap",
                ModifiedDate = DateTime.Now,
                UserRole = UserRoleList.Admin,
            };

            var manager = new ApplicationUser
            {
                FirstName = "manager",
                LastName = "manager",
                Email = "manager@impactleap.com",
                NormalizedEmail = "MANAGER@IMPACTLEAP.COM",
                UserName = "manager@impactleap.com",
                NormalizedUserName = "MANAGER@IMPACTLEAP.COM",
                PhoneNumber = "000-000-0000",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                CompanyName = "Impact Leap",
                ModifiedDate = DateTime.Now,
                UserRole = UserRoleList.Manager,
            };

            var temp = new ApplicationUser
            {
                FirstName = "temp",
                LastName = "user",
                Email = "tempuser@impactleap.com",
                NormalizedEmail = "TEMPUSER@IMPACTLEAP.COM",
                UserName = "tempuser@impactleap.com",
                NormalizedUserName = "TEMPUSER@IMPACTLEAP.COM",
                PhoneNumber = "000-000-0000",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                CompanyName = "Impact Leap",
                ModifiedDate = DateTime.Now,
                UserRole = UserRoleList.Unregistered
            };

            var test = new ApplicationUser
            {
                FirstName = "test",
                LastName = "user",
                Email = "testuser@impactleap.com",
                NormalizedEmail = "TESTUSER@IMPACTLEAP.COM",
                UserName = "testuser@impactleap.com",
                NormalizedUserName = "TESTUSER@IMPACTLEAP.COM",
                PhoneNumber = "000-000-0000",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                CompanyName = "Impact Leap",
                ModifiedDate = DateTime.Now,
                UserRole = UserRoleList.Member
            };

            if (!context.Users.Any(u => u.UserName == admin.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(admin, "Password1!");
                admin.PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser>(context);
                var result = userStore.CreateAsync(admin);

                await AssignRoles(isp, admin.Email, "Admin");
            }

            if (!context.Users.Any(u => u.UserName == manager.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(manager, "Password1!");
                manager.PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser>(context);
                var result = userStore.CreateAsync(manager);

                await AssignRoles(isp, manager.Email, "Manager");
            }

            if (!context.Users.Any(u => u.UserName == temp.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(temp, "Password1!");
                temp.PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser>(context);
                var result = userStore.CreateAsync(temp);

                await AssignRoles(isp, temp.Email, "Unregistered");
            }

            if (!context.Users.Any(u => u.UserName == test.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(test, "Password1!");
                test.PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser>(context);
                var result = userStore.CreateAsync(test);

                await AssignRoles(isp, test.Email, "Member");
            }

            await context.SaveChangesAsync();
        }

        public static async Task<IdentityResult> AssignRoles(IServiceProvider services, string username, string role)
        {
            UserManager<ApplicationUser> _userManager = services.GetService<UserManager<ApplicationUser>>();
            ApplicationUser user = await _userManager.FindByNameAsync(username);
            var result = await _userManager.AddToRoleAsync(user, role);

            return result;
        }
    }

}