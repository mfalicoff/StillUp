using FastEndpoints;
using StillUp.ApiService.Entities;
using StillUp.ApiService.Services;

namespace StillUp.ApiService.Endpoints.Monitors;

public class MonitorStreamEndpoint(MonitorChannel monitorChannel) : EndpointWithoutRequest
{
    private readonly MonitorChannel _monitorChannel = monitorChannel;

    public override void Configure()
    {
        Get("/api/monitors/stream");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.EventStreamAsync(AsStreamItems(ct), ct);
    }

    private async IAsyncEnumerable<StreamItem> AsStreamItems([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        await foreach (MonitorEntry entry in _monitorChannel.Reader.ReadAllAsync(ct))
        {
            yield return new StreamItem("monitor-entry", entry);
        }
    }
}
