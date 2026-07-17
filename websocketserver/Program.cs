using websocketserver.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<WebSocketConnectionManager>();
builder.Services.AddSingleton<NotificationPublisher>();
builder.Services.AddHostedService<NotificationPublisherWorker>();

var app = builder.Build();
var connectionManager = app.Services.GetRequiredService<WebSocketConnectionManager>();
app.UseWebSockets();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
connectionManager.Add(webSocket);

Console.WriteLine(
    $"Client connected. Total clients: {connectionManager.Connections.Count}");
        Console.WriteLine("WebSocket client connected.");

        var buffer = new byte[1024];

        while (webSocket.State == System.Net.WebSockets.WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None
            ); 

            if (result.MessageType == System.Net.WebSockets.WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(
                    System.Net.WebSockets.WebSocketCloseStatus.NormalClosure,
                    "Closed",
                    CancellationToken.None
                );

                break;
            }

            var message = System.Text.Encoding.UTF8.GetString(
                buffer,
                0,
                result.Count
            );

            Console.WriteLine($"Received: {message}");
        }
    connectionManager.Remove(webSocket);

Console.WriteLine(
    $"Client disconnected. Total clients: {connectionManager.Connections.Count}");
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});


app.Run();