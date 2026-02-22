namespace StillUp.ApiService.Services;

public interface IServiceMonitor
{
    Task MonitorService(CancellationToken ct);
}