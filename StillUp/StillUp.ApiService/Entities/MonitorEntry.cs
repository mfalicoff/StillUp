namespace StillUp.ApiService.Entities;

public record MonitorEntry(string Name, DateTime Date, string Url, string StatusCode)
{
    public int Id { get; init; }
}
