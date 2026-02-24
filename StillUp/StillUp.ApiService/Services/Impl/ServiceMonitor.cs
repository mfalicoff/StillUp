using Docker.DotNet.Models;
using StillUp.ApiService.Entities;
using StillUp.ApiService.Repositories;
using StillUp.ApiService.Services;

namespace StillUp.ApiService.Services.Impl;

public class ServiceMonitor(IDockerService dockerService, IMonitorEntryRepository monitorRepository, MonitorChannel monitorChannel): IServiceMonitor
{
    private readonly IDockerService _dockerService = dockerService;
    private readonly IMonitorEntryRepository _monitorRepository = monitorRepository;
    private readonly MonitorChannel _monitorChannel = monitorChannel;

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
                    catch (Exception e)
                    {
                        return null;
                    }
                })
                .Where(x => x != null)!;

            foreach (Monitor monitor in monitors)
            {
                
                HttpClient httpClient = new();
                httpClient.BaseAddress = new Uri(monitor.url);
                HttpResponseMessage res = await httpClient.GetAsync("/", ct);
            
                Console.WriteLine($"Pinged {monitor.name} at {monitor.url} at {TimeProvider.System.GetLocalNow()} with res: {res.StatusCode}");
                MonitorEntry entry = new(monitor.name, DateTime.UtcNow, monitor.url, res.StatusCode.ToString());
                await _monitorRepository.InsertAsync(entry, ct);
                await _monitorChannel.Writer.WriteAsync(entry, ct);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private record Monitor(string name, string url);
}