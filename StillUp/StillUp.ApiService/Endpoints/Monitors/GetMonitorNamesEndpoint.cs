using FastEndpoints;
using StillUp.ApiService.Repositories;

namespace StillUp.ApiService.Endpoints.Monitors;

public class GetMonitorNamesResponse
{
    public List<string> Names { get; set; } = [];
}

public class GetMonitorNamesEndpoint(IMonitorEntryRepository repository) : EndpointWithoutRequest<GetMonitorNamesResponse>
{
    private readonly IMonitorEntryRepository _repository = repository;

    public override void Configure()
    {
        Get("/api/monitors/names");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            IEnumerable<string> names = await _repository.GetDistinctNamesAsync(ct);
            await Send.OkAsync(new GetMonitorNamesResponse { Names = names?.ToList() ?? [] }, ct);
        }
        catch (OperationCanceledException)
        {
            // Client disconnected — normal
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to retrieve distinct monitor names from repository");
            throw;
        }
    }
}
