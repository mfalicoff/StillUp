using Docker.DotNet;

namespace StillUp.ApiService.Clients;

public class DockerClientWrapper
{
    public DockerClient Client = new DockerClientConfiguration().CreateClient();
}