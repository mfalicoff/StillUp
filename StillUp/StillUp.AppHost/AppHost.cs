IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("postgres")
    .WithImage("timescale/timescaledb", "latest-pg17")  // or latest-pg15, pg17
    .WithImageRegistry("docker.io")
    .WithPgWeb(); // optional

IResourceBuilder<PostgresDatabaseResource> db = postgres.AddDatabase("mydb");

IResourceBuilder<ProjectResource> apiService = builder.AddProject<Projects.StillUp_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WaitFor(db)
    .WithReference(db);

builder.Build().Run();
