namespace StillUp.ApiService.Services.Impl;

public class ServiceMonitor(IDockerService dockerService, TimescaleContext timescaleContext): IServiceMonitor
{
    private readonly IDockerService _dockerService = dockerService;
    private readonly TimescaleContext _timescaleContext = timescaleContext;

    public async Task MonitorService(CancellationToken ct)
    {
        try
        {
            var containers = await _dockerService.ListContainersAsync(ct);
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
                var res = await httpClient.GetAsync("/", ct);
            
                Console.WriteLine($"Pinged {monitor.name} at {monitor.url} at {TimeProvider.System.GetLocalNow()} with res: {res.StatusCode}");
                _timescaleContext.MonitorEntries.Add(new MonitorEntry(monitor.name, DateTime.UtcNow, monitor.url, res.StatusCode.ToString() ));
                await _timescaleContext.SaveChangesAsync(ct);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        
        /**
         * getkey stillup.name
         * getkey stillup.url
         * {
         *  servicename,
         *  url
         * }
         */
    }

    private record Monitor(string name, string url);
}