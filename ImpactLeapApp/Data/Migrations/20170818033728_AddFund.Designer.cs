using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ImpactLeapApp.Data;
using ImpactLeapApp.Models;
using ImpactLeapApp.Models.OrderModels;

namespace WebApplication1.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20170818033728_AddFund")]
    partial class AddFund
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ImpactLeapApp.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<int?>("BillingAddressId");

                    b.Property<string>("CompanyName");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .HasMaxLength(20);

                    b.Property<string>("LastName")
                        .HasMaxLength(20);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.Property<int>("UserRole");

                    b.HasKey("Id");

                    b.HasIndex("BillingAddressId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("ImpactLeapApp.Models.BillingModels.BillingAddress", b =>
                {
                    b.Property<int>("BillingAddressId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddressLine1")
                        .HasMaxLength(50);

                    b.Property<string>("AddressLine2")
                        .HasMaxLength(50);

                    b.Property<string>("BillingName");

                    b.Property<string>("City")
                        .HasMaxLength(50);

                    b.Property<string>("Country");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("State");

                    b.Property<string>("UserId");

                    b.Property<string>("ZipCode")
                        .HasMaxLength(10);

                    b.HasKey("BillingAddressId");

                    b.ToTable("BillingAddresses");
                });

            modelBuilder.Entity("ImpactLeapApp.Models.Fund", b =>
                {
                    b.Property<int>("FundId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("FundManager");

                    b.Property<string>("FundName");

                    b.Property<string>("Geography");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Strategy");

                    b.HasKey("FundId");

                    b.ToTable("Funds");
                });

            modelBuilder.Entity("ImpactLeapApp.Models.OrderModels.Module", b =>
                {
                    b.Property<int>("ModuleId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("ModuleName");

                    b.Property<int>("UnitPrice");

                    b.HasKey("ModuleId");

                    b.ToTable("Modules");
                });

            modelBuilder.Entity("ImpactLeapApp.Models.OrderModels.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicationUserId");

                    b.Property<DateTime?>("DeliveredDate");

                    b.Property<int>("InvestmentId");

                    b.Property<bool>("IsPromotionCodeApplied");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("ModuleIds");

                    b.Property<string>("NoteFromAdmin");

                    b.Property<string>("NoteFromUser");

                    b.Property<string>("OrderNum");

                    b.Property<int>("OrderStatus");

                    b.Property<DateTime>("OrderedDate");

                    b.Property<int>("PromotionId");

                    b.Property<string>("SalesRep")
                        .HasMaxLength(160);

                    b.Property<int>("SelectionDiscount");

                    b.Property<int>("TotalAmount");

                    b.Property<string>("UploadedFileName");

                    b.Property<string>("UploadedFilePath");

                    b.Property<string>("UserEmail")
                        .IsRequired();

                    b.Property<string>("UserId");

                    b.HasKey("OrderId");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("ImpactLeapApp.Models.OrderModels.OrderDetail", b =>
                {
                    b.Property<int>("OrderDetailId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<int>("ModuleId");

                    b.Property<string>("ModuleName");

                    b.Property<int>("OrderId");

                    b.HasKey("OrderDetailId");

                    b.HasIndex("ModuleId");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("ImpactLeapApp.Models.OrderModels.Promotion", b =>
                {
                    b.Property<int>("PromotionId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateFrom");

                    b.Property<DateTime>("DateTo");

                    b.Property<string>("Description");

                    b.Property<int>("DiscountMethod");

                    b.Property<int>("DiscountRate");

                    b.Property<bool>("IsActive");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("PromotionCode")
                        .IsRequired()
                        .HasMaxLength(8);

                    b.Property<string>("PromotionName")
                        .HasMaxLength(160);

                    b.HasKey("PromotionId");

                    b.ToTable("Promotions");
                });

            modelBuilder.Entity("ImpactLeapApp.Models.OrderModels.Saving", b =>
                {
                    b.Property<int>("SavingId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<int>("DiscountMethod");

                    b.Property<bool>("IsActive");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("SavingName");

                    b.Property<int>("SavingRate");

                    b.Property<int>("SelectFrom");

                    b.Property<int>("SelectTo");

                    b.HasKey("SavingId");

                    b.ToTable("Savings");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("ImpactLeapApp.Models.ApplicationUser", b =>
                {
                    b.HasOne("ImpactLeapApp.Models.BillingModels.BillingAddress", "BillingAddress")
                        .WithMany()
                        .HasForeignKey("BillingAddressId");
                });

            modelBuilder.Entity("ImpactLeapApp.Models.OrderModels.Order", b =>
                {
                    b.HasOne("ImpactLeapApp.Models.ApplicationUser")
                        .WithMany("Orders")
                        .HasForeignKey("ApplicationUserId");
                });

            modelBuilder.Entity("ImpactLeapApp.Models.OrderModels.OrderDetail", b =>
                {
                    b.HasOne("ImpactLeapApp.Models.OrderModels.Module", "Module")
                        .WithMany()
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("ImpactLeapApp.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ImpactLeapApp.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ImpactLeapApp.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
