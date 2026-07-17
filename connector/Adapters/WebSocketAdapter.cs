using System.Text.Json;
using System.Net.WebSockets;
using System.Text;
using connector.Contracts;
using connector.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using connector.Options;
namespace connector.Adapters;

public class WebSocketAdapter : ISourceAdapter
{
    
    private readonly ClientWebSocket _socket = new();
private readonly ILogger<WebSocketAdapter> _logger;
private readonly ConnectorWebSocketOptions _options;

public WebSocketAdapter(
    ILogger<WebSocketAdapter> logger,
    IOptions<ConnectorWebSocketOptions> options)
{
    _logger = logger;
    _options = options.Value;
}
    

    public string Name => "websocket";

    public event Func<RawMessage, Task>? OnRawMessage;

    public async Task ConnectAsync(CancellationToken cancellationToken)
{
    _logger.LogInformation(
        "Connecting to WebSocket server: {Url}",
        _options.Url);

    await _socket.ConnectAsync(
        new Uri(_options.Url),
        cancellationToken);

    _logger.LogInformation("Connected to WebSocket server.");
var buffer = new byte[4096];

while (_socket.State == WebSocketState.Open &&
       !cancellationToken.IsCancellationRequested)
{
    var result = await _socket.ReceiveAsync(
        new ArraySegment<byte>(buffer),
        cancellationToken);

    if (result.MessageType == WebSocketMessageType.Close)
    {
        _logger.LogInformation("WebSocket server closed the connection.");
        break;
    }

    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

    _logger.LogInformation("Received: {Message}", message);
NotificationMessage? notification;

try
{
    notification = JsonSerializer.Deserialize<NotificationMessage>(message);
}
catch (JsonException ex)
{
    _logger.LogWarning(ex, "Invalid WebSocket message.");
    continue;
}

if (notification == null)
    continue;

if (OnRawMessage != null)
{
    await OnRawMessage.Invoke(new RawMessage
    {
        Source = notification.Source,
        Payload = notification.Message,
        ReceivedAt = DateTime.UtcNow
    });
}
}



}

    public async Task DisconnectAsync(CancellationToken cancellationToken)
{
    if (_socket.State == WebSocketState.Open)
    {
        await _socket.CloseAsync(
            WebSocketCloseStatus.NormalClosure,
            "Connector stopping",
            cancellationToken);
    }
}
}