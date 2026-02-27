namespace StillUp.ApiService.Entities;

public class MonitorState(string name, string url, bool isHealthy, string statusCode)
{
    public int Id { get; init; }

    public string Name { get; init; } = name;

    public string Url { get; set; } = url;

    public bool IsHealthy { get; set; } = isHealthy;

    public string StatusCode { get; set; } = statusCode;

    public DateTime LastChecked { get; set; } = DateTime.UtcNow;

    public DateTime LastChanged { get; set; } = DateTime.UtcNow;
}
