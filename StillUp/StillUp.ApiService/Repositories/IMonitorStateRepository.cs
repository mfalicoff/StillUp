using StillUp.ApiService.Entities;

namespace StillUp.ApiService.Repositories;

public interface IMonitorStateRepository
{
    Task<MonitorState?> GetByNameAsync(string name, CancellationToken ct = default);

    /// <summary>
    /// Upserts the monitor state. Returns true if health state changed (and a notification should be sent).
    /// Returns false on first observation or when health state is unchanged.
    /// </summary>
    Task<bool> UpsertAsync(string name, string url, bool isHealthy, string statusCode, CancellationToken ct = default);
}
