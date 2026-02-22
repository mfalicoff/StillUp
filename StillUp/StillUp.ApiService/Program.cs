using CmdScale.EntityFrameworkCore.TimescaleDB;
using Docker.DotNet;
using Docker.DotNet.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StillUp.ApiService.Extensions;
using StillUp.ApiService.Workers;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddFastEndpoints();


builder.Services.AddDockerServices();
builder.Services.AddMonitors();
builder.Services.AddHostedService<MonitorWorker>();

builder.AddNpgsqlDbContext<TimescaleContext>(connectionName: "mydb", configureDbContextOptions: options =>
{
    options.UseTimescaleDb();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapDefaultEndpoints();
app.UseFastEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TimescaleContext>();
    db.Database.Migrate();
}

app.Run();

public class TimescaleContext(DbContextOptions<TimescaleContext> options) : DbContext(options)
{
    public DbSet<MonitorEntry> MonitorEntries { get; set; }
}

public class MonitorEntry
{
    public MonitorEntry(string name, DateTime date, string url, string statusCode)
    {
        Name = name;
        Date = date;
        Url = url;
        StatusCode = statusCode;
    }

    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Url {get; set;}
    
    public DateTime Date { get; set; }
    
    public string StatusCode { get; set; }
    
}
