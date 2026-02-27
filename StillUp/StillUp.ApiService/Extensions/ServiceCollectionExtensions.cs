using StillUp.ApiService.Clients;
using StillUp.ApiService.Notifications;
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
            services.AddTransient<IMonitorStateRepository, MonitorStateRepository>();
            return services;
        }

        public IServiceCollection AddMonitors()
        {
            services.AddSingleton<MonitorChannel>();
            services.AddHttpClient("monitor");
            services.AddTransient<IServiceMonitor, ServiceMonitor>();

            return services;
        }

        public IServiceCollection AddNotifications(IConfiguration configuration)
        {
            services.AddHttpClient<INtfyNotifier, NtfyNotifier>(client =>
            {
                string baseUrl = configuration["Ntfy:BaseUrl"] ?? "https://ntfy.sh";
                client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
            });

            return services;
        }
    }
}