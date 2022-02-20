using FPSE.Core;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class CoreSetup
    {
        public static IServiceCollection AddFlightDataCollection(this IServiceCollection services)
        {
            services.AddSingleton<IDownloadScheduler, DownloadScheduler>();
            services.AddSingleton<IFlightDataCache, FlightDataCache>();

            return services;
        }
    }
}