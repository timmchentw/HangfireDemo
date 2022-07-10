using Hangfire;
using Hangfire.RecurringJobAdmin;
using Hangfire.SqlServer;
using HangfireDemo.Data;
using HangfireDemo.Helpers;
using HangfireDemo.Helpers.Hangfire;
using HangfireDemo.Services;
using HangfireDemo.Shared;
using HangfireDemo.Shared.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HangfireDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string defaultConnectionString = Configuration.GetConnectionString("DefaultConnection");

            services.Configure<HangfireConfig>(Configuration.GetSection(nameof(HangfireConfig)));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(defaultConnectionString));
            services.AddDatabaseDeveloperPageExceptionFilter();

            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(defaultConnectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                })
                .UseRecurringJobAdmin(typeof(Startup).Assembly));

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)    // Do not use email verifivation
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews()
                    .AddNewtonsoftJson();   // This package can parse nullable object of parameter

            // Register services
            services.AddTransient<IWebsiteJobService, WebsiteJobServiceDemo>();
            services.AddTransient<IConsoleJobService, ConsoleJobServiceDemo>();
            services.AddSingleton<StartupValues>(new StartupValues() { ProgramNamespace = MethodBase.GetCurrentMethod().DeclaringType.Namespace });

            // Add OpenAPI v3 document (Swagger)
            services.AddOpenApiDocument(configure =>
            {
                configure.Title = "Hangfire Demo Job Api";
                configure.Description = @"Generate, Enqueue, Delete Hangfire Jobs by Calling the APIs";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHangfireDashboard(new DashboardOptions()
                {
                    Authorization = new[] { new UserAuthorizationFilter() },   // Only authorized user can access
                    AppPath = null, // Do not show "Go back" button in dashboard page
                });
            });

            // Add OpenAPI v3 document (Swagger)
            app.UseOpenApi();
            app.UseSwaggerUi3();

            // Custom hangfire settings
            var hanfireConfig = app.ApplicationServices.GetService<IOptions<HangfireConfig>>().Value;
            GlobalJobFilters.Filters.Add(new ProlongExpirationTimeAttribute(new TimeSpan(hanfireConfig.ProlongExpirationDays, 0, 0, 0)));
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute()
            {
                 Attempts = hanfireConfig.MaxRetryAttempts,
                 DelaysInSeconds = hanfireConfig.DelaysInSeconds
            });
        }
    }
}
