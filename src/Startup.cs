using FlightPlanner.Service.Filters;
using FlightPlanner.Service.Middlewares;
using FlightPlanner.Service.Repositories;
using FlightPlanner.Service.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Insights;
using Service.Middlewares;
using Service.Tasks;
using System;

namespace FlightPlanner.Service
{
    public class Startup
    {
        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment environment
            )
        {
            Configuration = configuration;
            Environment = environment;
        }


        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get;  }


        public void ConfigureServices(IServiceCollection services)
        {
            // For session temp data
            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddSession();

            // Razor pages initialization
            services.AddRazorPages().AddMvcOptions(options =>
            {
                options.Filters.Add(new OrganizationFilter());
                options.Filters.Add(new EssentialCookieFilter());
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            });

            // Health checks and error reporting
            if (Environment.IsProduction())
            {
                services.AddApplicationInsights();
            }

            services.AddHealthChecks()
                .AddCheck<TaskHealthCheck>("TaskHealthCheck");

            // Initialization code
            services.AddSingleton<ITaskScheduler, TaskScheduler>();
            services.AddSingleton<IFlightDataRepository, FlightDataRepository>();
            services.AddSingleton<INotificationRepository, NotificationRepository>();
            services.AddTransient<IStartupFilter, WebAppInit>();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSecurityHeaders();
            app.UseNoResponseCaching();
            app.UseScriptBlocking();
            app.UseHttpsRedirection();
            app.UseSession();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
