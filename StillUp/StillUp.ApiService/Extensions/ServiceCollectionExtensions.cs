using StillUp.ApiService.Clients;
using StillUp.ApiService.Repositories;
using StillUp.ApiService.Repositories.Impl;
using StillUp.ApiService.Services;
using StillUp.ApiService.Services.Impl;

namespace StillUp.ApiService.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDockerServices()
        {
            services.AddSingleton<DockerClientWrapper>();
            services.AddTransient<IDockerService, DockerService>();

            return services;
        }

        public IServiceCollection AddRepositories()
        {
            services.AddTransient<IMonitorEntryRepository, MonitorEntryRepository>();
            return services;
        }

        public IServiceCollection AddMonitors()
        {
            services.AddSingleton<MonitorChannel>();
            services.AddTransient<IServiceMonitor, ServiceMonitor>();
            
            return services;
        }
    }
}