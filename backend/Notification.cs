namespace backend.Models;

public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Bildirim hangi kaynaktan geldi?
    public string Source { get; set; } = "";

    public string Title { get; set; } = "";

    public string Message { get; set; } = "";

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}