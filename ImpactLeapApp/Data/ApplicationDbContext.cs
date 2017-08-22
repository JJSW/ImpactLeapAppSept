using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ImpactLeapApp.Models;
using ImpactLeapApp.Models.OrderModels;
using ImpactLeapApp.Models.BillingModels;

namespace ImpactLeapApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        // User models
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<BillingAddress> BillingAddresses { get; set; }

        // Order models
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Saving> Savings { get; set; }

        public DbSet<Module> Modules { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
    }
}
