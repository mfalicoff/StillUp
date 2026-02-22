using Docker.DotNet.Models;

namespace StillUp.ApiService.Services;

public interface IDockerService
{
    Task<IList<ContainerListResponse>> ListContainersAsync(CancellationToken ct); 
}