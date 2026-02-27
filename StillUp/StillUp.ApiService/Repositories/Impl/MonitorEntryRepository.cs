using Microsoft.EntityFrameworkCore;
using StillUp.ApiService.Data;
using StillUp.ApiService.Entities;

namespace StillUp.ApiService.Repositories.Impl;

public class MonitorEntryRepository(TimescaleContext context) : IMonitorEntryRepository
{
    private readonly TimescaleContext _context = context;

    public async Task InsertAsync(MonitorEntry entry, CancellationToken ct = default)
    {
        _context.MonitorEntries.Add(entry);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<MonitorEntry>> GetRecentEntriesAsync(string serviceName, int count = 100, CancellationToken ct = default)
    {
        return await _context.MonitorEntries
            .Where(e => e.Name == serviceName)
            .OrderByDescending(e => e.Date)
            .Take(count)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<MonitorEntry>> GetEntriesForLastHoursAsync(int hours, string? serviceName = null, CancellationToken ct = default)
    {
        DateTime since = DateTime.UtcNow.AddHours(-hours);
        IQueryable<MonitorEntry> query = _context.MonitorEntries.Where(e => e.Date >= since);
        
        if (!string.IsNullOrEmpty(serviceName))
        {
            query = query.Where(e => e.Name == serviceName);
        }

        return await query.OrderByDescending(e => e.Date).ToListAsync(ct);
    }

    public async Task<IEnumerable<string>> GetDistinctNamesAsync(CancellationToken ct = default)
    {
        return await _context.MonitorEntries
            .Select(e => e.Name)
            .Distinct()
            .OrderBy(n => n)
            .ToListAsync(ct);
    }
}
