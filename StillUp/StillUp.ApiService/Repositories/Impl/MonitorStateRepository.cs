using Microsoft.EntityFrameworkCore;
using StillUp.ApiService.Data;
using StillUp.ApiService.Entities;

namespace StillUp.ApiService.Repositories.Impl;

public class MonitorStateRepository(TimescaleContext context) : IMonitorStateRepository
{
    private readonly TimescaleContext _context = context;

    public async Task<MonitorState?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.MonitorStates.FirstOrDefaultAsync(s => s.Name == name, ct);
    }

    public async Task<bool> UpsertAsync(string name, string url, bool isHealthy, string statusCode, CancellationToken ct = default)
    {
        MonitorState? existing = await _context.MonitorStates.FirstOrDefaultAsync(s => s.Name == name, ct);

        if (existing is null)
        {
            _context.MonitorStates.Add(new MonitorState(name, url, isHealthy, statusCode));
            await _context.SaveChangesAsync(ct);
            return false;
        }

        bool stateChanged = existing.IsHealthy != isHealthy;
        existing.Url = url;
        existing.IsHealthy = isHealthy;
        existing.StatusCode = statusCode;
        existing.LastChecked = DateTime.UtcNow;

        if (stateChanged)
            existing.LastChanged = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return stateChanged;
    }
}
