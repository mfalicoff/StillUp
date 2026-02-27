using FastEndpoints;
using StillUp.ApiService.Entities;
using StillUp.ApiService.Repositories;

namespace StillUp.ApiService.Endpoints.Monitors;

public class GetRecentMonitorDataRequest
{
    public int Hours { get; set; }
    public string? ServiceName { get; set; }
}

public class GetRecentMonitorDataResponse
{
    public GetRecentMonitorDataResponse(List<MonitorEntry> data)
    {
        Dictionary<string, List<MonitorEntry>> entries = [];

        IEnumerable<(string Key, List<MonitorEntry>)> grouping = data.GroupBy(x => x.Name).Select(x => (x.Key, x.ToList()));
        foreach ((string Key, List<MonitorEntry>) valueTuple in grouping)
        {
            entries.Add(valueTuple.Key, valueTuple.Item2);
        }

        Entries = entries;
    }

    public Dictionary<string, List<MonitorEntry>> Entries { get; set; }
}

public class GetRecentMonitorDataEndpoint(IMonitorEntryRepository repository) : Endpoint<GetRecentMonitorDataRequest, GetRecentMonitorDataResponse>
{
    private readonly IMonitorEntryRepository _repository = repository;

    public override void Configure()
    {
        Get("/api/monitors/recent/{Hours}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetRecentMonitorDataRequest req, CancellationToken ct)
    {
        int hoursToFetch = req.Hours > 0 ? req.Hours : 1;
        IEnumerable<MonitorEntry> data = await _repository.GetEntriesForLastHoursAsync(hoursToFetch, req.ServiceName, ct);
        
        await Send.OkAsync(new GetRecentMonitorDataResponse(data.ToList()), ct);
    }
}
