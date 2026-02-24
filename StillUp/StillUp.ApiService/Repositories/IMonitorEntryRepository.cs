using StillUp.ApiService.Entities;

namespace StillUp.ApiService.Repositories;

public interface IMonitorEntryRepository
{
    Task InsertAsync(MonitorEntry entry, CancellationToken ct = default);
    
    Task<IEnumerable<MonitorEntry>> GetRecentEntriesAsync(string serviceName, int count = 100, CancellationToken ct = default);
    
    Task<IEnumerable<MonitorEntry>> GetEntriesForLastHoursAsync(int hours, string? serviceName = null, CancellationToken ct = default);
}
