namespace connector.Models;

public class RawMessage
{
    public string Source { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
}