namespace StillUp.ApiService.Entities;

public class MonitorEntry
{
    public MonitorEntry(string name, DateTime date, string url, string statusCode)
    {
        Name = name;
        Date = date;
        Url = url;
        StatusCode = statusCode;
    }

    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Url { get; set; }
    
    public DateTime Date { get; set; }
    
    public string StatusCode { get; set; }
}
