using websocketserver.Models;

namespace websocketserver.Services;

public class NotificationPublisher
{
    private readonly WebSocketConnectionManager _connectionManager;

    public NotificationPublisher(WebSocketConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public async Task PublishTestNotificationAsync()
    {
       Console.WriteLine($"Publishing notification at {DateTime.Now:HH:mm:ss}");
       var notification = new NotificationMessage
        {
            Source = "websocket",
            Title = "WebSocket Test",
            Message = $"Test message - {DateTime.Now:HH:mm:ss}"
        };

        await _connectionManager.BroadcastAsync(notification);
    }
}