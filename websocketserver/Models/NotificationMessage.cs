namespace websocketserver.Models;

public class NotificationMessage
{
    public string Source { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}