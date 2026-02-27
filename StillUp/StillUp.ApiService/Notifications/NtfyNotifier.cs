namespace StillUp.ApiService.Notifications;

public interface INtfyNotifier
{
    Task NotifyFailureAsync(string name, string url, string statusCode, CancellationToken ct = default);
    Task NotifyRecoveryAsync(string name, string url, CancellationToken ct = default);
}

public class NtfyNotifier(HttpClient httpClient, IConfiguration configuration) : INtfyNotifier
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _topic = configuration["Ntfy:Topic"] ?? "stillup";

    public async Task NotifyFailureAsync(string name, string url, string statusCode, CancellationToken ct = default)
    {
        using HttpRequestMessage request = new(HttpMethod.Post, _topic);
        request.Content = new StringContent($"{name} at {url} responded with {statusCode}");
        request.Headers.Add("Title", $"{name} is down");
        request.Headers.Add("Priority", "urgent");
        request.Headers.Add("Tags", "warning,skull");
        await _httpClient.SendAsync(request, ct);
    }

    public async Task NotifyRecoveryAsync(string name, string url, CancellationToken ct = default)
    {
        using HttpRequestMessage request = new(HttpMethod.Post, _topic);
        request.Content = new StringContent($"{name} at {url} is back up");
        request.Headers.Add("Title", $"{name} recovered");
        request.Headers.Add("Priority", "default");
        request.Headers.Add("Tags", "white_check_mark");
        await _httpClient.SendAsync(request, ct);
    }
}
