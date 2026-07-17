namespace connector.Models;

public class NotificationEnvelope
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Source { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}