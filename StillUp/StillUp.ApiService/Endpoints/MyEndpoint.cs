using Docker.DotNet.Models;
using FastEndpoints;
using StillUp.ApiService.Services;

namespace StillUp.ApiService.Endpoints;

public class MyEndpoint(IDockerService dockerService) : EndpointWithoutRequest<MyResponse>
{
    private readonly IDockerService _dockerService = dockerService;

    public override void Configure()
    {
        Get("/api/containers/running");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        IList<ContainerListResponse> containers = await _dockerService.ListContainersAsync(ct);
        await Send.OkAsync(new MyResponse
        {
           Containers = containers.ToList()
        }, ct);
    }
}


public class MyResponse
{
    public required List<ContainerListResponse> Containers { get; set; }
}
