using CmdScale.EntityFrameworkCore.TimescaleDB;
using CmdScale.EntityFrameworkCore.TimescaleDB.Configuration.Hypertable;
using Microsoft.EntityFrameworkCore;
using StillUp.ApiService.Entities;

namespace StillUp.ApiService.Data;

public class TimescaleContext(DbContextOptions<TimescaleContext> options) : DbContext(options)
{
    public DbSet<MonitorEntry> MonitorEntries { get; set; }
    public DbSet<MonitorState> MonitorStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MonitorEntry>()
            .IsHypertable(x => x.Date);

        modelBuilder.Entity<MonitorState>()
            .HasIndex(s => s.Name)
            .IsUnique();
    }
}
