using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using ImpactLeapApp.Configuration;
using ImpactLeapApp.Data;
using ImpactLeapApp.Models;
using ImpactLeapApp.Models.BillingModels;
using ImpactLeapApp.Models.SampleSeedData;
using ImpactLeapApp.Services;
using Stripe;
using Microsoft.AspNetCore.Owin;
using Microsoft.AspNetCore.Authentication.Cookies;
using ImpactLeapApp.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace ImpactLeapApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Use in memory database for unit testing
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase());

            services.AddIdentity<ApplicationUser, IdentityRole>(identity =>
            {
                // Configure identity options
                identity.Password.RequireDigit = true;
                identity.Password.RequireLowercase = true;
                identity.Password.RequireUppercase = true;
                identity.Password.RequireNonAlphanumeric = true;
                identity.Password.RequiredLength = 6;
            })

            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // User session for tempdata
            services.AddMemoryCache();
            services.AddSession();
            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            // Add Auth
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.Requirements.Add(new UserNamesRequirement("admin@impactleap.com", "manager@impactleap.com")));
            });
            services.AddSingleton<IAuthorizationHandler, UserNamesHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ApplicationDbContext context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseIdentity();
            app.UseSession();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}/{*more}");
            });

            // Set stripe key to Stripe config so that it can be called.
            StripeConfiguration.SetApiKey(Configuration.GetSection("Stripe")["SecretKey"]);

            SeedData.Initialize(context);
            RoleSeedData.Initialize(app.ApplicationServices);


            // Auth
            app.UseCookieAuthentication();
            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                ClientId = Configuration["AzureAd:ClientId"],
                Authority = string.Format(Configuration["AzureAd:AadInstance"], Configuration["AzureAd:TenantId"]),
                CallbackPath = Configuration["AzureAd:AuthCallback"]
            });

        }
    }

}
