var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImage("timescale/timescaledb", "latest-pg17")  // or latest-pg15, pg17
    .WithImageRegistry("docker.io")
    .WithPgAdmin(); // optional

var db = postgres.AddDatabase("mydb");

var apiService = builder.AddProject<Projects.StillUp_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WaitFor(db)
    .WithReference(db);

builder.Build().Run();
