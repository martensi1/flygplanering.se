using FlightPlanner.Core;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class CoreSetup
    {
        public static IServiceCollection AddFlightDataCollection(this IServiceCollection services)
        {
            services.AddSingleton<ITaskScheduler, TaskScheduler>();
            services.AddSingleton<IFlightDataSource, FlightDataSource>();

            return services;
        }
    }
}