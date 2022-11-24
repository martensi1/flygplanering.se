using FlightPlanner.Service.Filters;
using FlightPlanner.Service.Repositories;
using FlightPlanner.Service.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace FlightPlanner.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            // For temp data
            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddSession();

            // Razor pages initialization
            services.AddRazorPages().AddMvcOptions(options =>
            {
                options.Filters.Add(new RejectFilter());
                options.Filters.Add(new OrganizationFilter());
                options.Filters.Add(new NoCacheFilter());
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            });

            // Initialization code
            services.AddSingleton<ITaskScheduler, TaskScheduler>();
            services.AddSingleton<IFlightDataRepository, FlightDataRepository>();
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

            app.UseCookiePolicy();
            app.UseHttpsRedirection();
            app.UseSession();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
