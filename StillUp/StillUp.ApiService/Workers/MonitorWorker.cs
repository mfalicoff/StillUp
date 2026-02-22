using StillUp.ApiService.Services;

namespace StillUp.ApiService.Workers;

public class MonitorWorker(IServiceScopeFactory factory): BackgroundService
{
    private readonly IServiceScopeFactory _factory = factory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            var scope = _factory.CreateAsyncScope();
            var monitor = scope.ServiceProvider.GetRequiredService<IServiceMonitor>();
            await monitor.MonitorService(stoppingToken);
            
            await Task.Delay(5000, stoppingToken);
        }
    }
}