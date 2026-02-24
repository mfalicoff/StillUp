using Docker.DotNet.Models;
using StillUp.ApiService.Clients;

namespace StillUp.ApiService.Services.Impl;

public class DockerService(DockerClientWrapper clientWrapper): IDockerService
{
    private readonly DockerClientWrapper _clientWrapper = clientWrapper;

    public async Task<IList<ContainerListResponse>> ListContainersAsync(CancellationToken ct)
    {
        return await _clientWrapper.Client.Containers.ListContainersAsync(
            new ContainersListParameters {
                Limit = 10,
                // Filters = new Dictionary<string, IDictionary<string, bool>>
                // {
                //     { "status", new Dictionary<string, bool> { { "running", true } } }
                // }
            }, ct);
    }
}