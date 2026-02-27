using CmdScale.EntityFrameworkCore.TimescaleDB;
using Docker.DotNet;
using Docker.DotNet.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StillUp.ApiService.Extensions;
using StillUp.ApiService.Workers;
using StillUp.ApiService.Data;
using StillUp.ApiService.Entities;
using StillUp.ServiceDefaults;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();
builder.Services.AddFastEndpoints();

builder.Services.AddDockerServices();
builder.Services.AddRepositories();
builder.Services.AddMonitors();
builder.Services.AddNotifications(builder.Configuration);
builder.Services.AddHostedService<MonitorWorker>();

builder.AddNpgsqlDbContext<TimescaleContext>(connectionName: "mydb", configureDbContextOptions: options =>
{
    options.UseTimescaleDb();
});

WebApplication app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapDefaultEndpoints();
app.UseFastEndpoints();

using (IServiceScope scope = app.Services.CreateScope())
{
    TimescaleContext db = scope.ServiceProvider.GetRequiredService<TimescaleContext>();
    db.Database.Migrate();
}

app.Run();

