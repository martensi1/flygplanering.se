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
            services.AddRazorPages();

            services.AddTransient<IStartupFilter, WebAppInit>();
            services.AddFlightDataCollection();
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            AddNoCacheHeaders(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

        private void AddNoCacheHeaders(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers[HeaderNames.CacheControl] = "no-cache, no-store, must-revalidate";
                context.Response.Headers[HeaderNames.Expires] = "0";
                context.Response.Headers[HeaderNames.Pragma] = "no-cache";

                await next.Invoke();
            }
            );
        }
    }
}
