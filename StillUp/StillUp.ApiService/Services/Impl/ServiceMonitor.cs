using Docker.DotNet.Models;
using StillUp.ApiService.Entities;
using StillUp.ApiService.Notifications;
using StillUp.ApiService.Repositories;
using StillUp.ApiService.Services;

namespace StillUp.ApiService.Services.Impl;

public class ServiceMonitor(
    IDockerService dockerService,
    IMonitorEntryRepository monitorRepository,
    IMonitorStateRepository stateRepository,
    MonitorChannel monitorChannel,
    INtfyNotifier notifier,
    IHttpClientFactory httpClientFactory) : IServiceMonitor
{
    private readonly IDockerService _dockerService = dockerService;
    private readonly IMonitorEntryRepository _monitorRepository = monitorRepository;
    private readonly IMonitorStateRepository _stateRepository = stateRepository;
    private readonly MonitorChannel _monitorChannel = monitorChannel;
    private readonly INtfyNotifier _notifier = notifier;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task MonitorService(CancellationToken ct)
    {
        try
        {
            IList<ContainerListResponse> containers = await _dockerService.ListContainersAsync(ct);
            IEnumerable<Monitor> monitors = containers
                .Select(x =>
                {
                    try
                    {
                        string name = x.Labels["stillup.name"];
                        string url = x.Labels["stillup.url"];
                        return new Monitor(name, url);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                })
                .Where(x => x != null)!;

            foreach (Monitor monitor in monitors)
            {
                string statusCode;
                bool isHealthy;

                try
                {
                    HttpClient httpClient = _httpClientFactory.CreateClient("monitor");
                    HttpResponseMessage res = await httpClient.GetAsync(monitor.url, ct);
                    statusCode = res.StatusCode.ToString();
                    isHealthy = res.IsSuccessStatusCode;
                    Console.WriteLine($"Pinged {monitor.name} at {monitor.url} at {TimeProvider.System.GetLocalNow()} with res: {res.StatusCode}");
                }
                catch (HttpRequestException)
                {
                    statusCode = "ConnectionFailure";
                    isHealthy = false;
                    Console.WriteLine($"Failed to reach {monitor.name} at {monitor.url} at {TimeProvider.System.GetLocalNow()}");
                }

                MonitorEntry entry = new(monitor.name, DateTime.UtcNow, monitor.url, statusCode);
                await _monitorRepository.InsertAsync(entry, ct);
                await _monitorChannel.Writer.WriteAsync(entry, ct);

                bool stateChanged = await _stateRepository.UpsertAsync(monitor.name, monitor.url, isHealthy, statusCode, ct);
                if (stateChanged)
                {
                    if (!isHealthy)
                        await _notifier.NotifyFailureAsync(monitor.name, monitor.url, statusCode, ct);
                    else
                        await _notifier.NotifyRecoveryAsync(monitor.name, monitor.url, ct);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private record Monitor(string name, string url);
}
