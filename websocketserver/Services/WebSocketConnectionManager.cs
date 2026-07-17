using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using websocketserver.Models;
namespace websocketserver.Services;

public class WebSocketConnectionManager
{
    private readonly List<WebSocket> _connections = new();

    public void Add(WebSocket socket)
    {
        _connections.Add(socket);
    }

    public void Remove(WebSocket socket)
    {
        _connections.Remove(socket);
    }

    public IReadOnlyCollection<WebSocket> Connections
        => _connections.AsReadOnly();
public async Task BroadcastAsync(NotificationMessage message)
{
    var json = JsonSerializer.Serialize(message);
    var bytes = Encoding.UTF8.GetBytes(json);

    foreach (var socket in _connections)
    {
        if (socket.State != WebSocketState.Open)
            continue;

        await socket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }
}
}