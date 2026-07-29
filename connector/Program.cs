

using connector;
using connector.Options;
using connector.Adapters;
using connector.Contracts;
using connector.Core;
using connector.Services;
using connector.Models;


var builder = WebApplication.CreateBuilder(args);
var connectorOptions = builder.Configuration
    .GetSection("Connector")
    .Get<ConnectorOptions>() ?? new ConnectorOptions();
// Adapter'lar

if (connectorOptions.EnabledAdapters.Contains("Webhook"))
{
    builder.Services.AddSingleton<ISourceAdapter, WebhookAdapter>();
}

if (connectorOptions.EnabledAdapters.Contains("WebSocket"))
{
    builder.Services.AddSingleton<ISourceAdapter, WebSocketAdapter>();
}

if (connectorOptions.EnabledAdapters.Contains("RabbitMQ"))
{
    builder.Services.AddSingleton<ISourceAdapter, RabbitMqAdapter>();
}

if (connectorOptions.EnabledAdapters.Contains("Redis"))
{
    builder.Services.AddSingleton<ISourceAdapter, RedisAdapter>();
}

// Core
builder.Services.AddSingleton<IConnector, ConnectorCore>();

// Backend Sender
builder.Services.AddHttpClient<BackendSenderService>((serviceProvider, client) =>

{

    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    client.BaseAddress = new Uri(configuration["Backend:Url"]!);

});

builder.Services.AddHttpClient<IBackendSender, BackendSenderService>((serviceProvider, client) =>

{

    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    client.BaseAddress = new Uri(configuration["Backend:Url"]!);

});

builder.Services.Configure<ConnectorWebSocketOptions>(
    builder.Configuration.GetSection("WebSocket"));
builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<RedisOptions>(
    builder.Configuration.GetSection("Redis"));
builder.Services.Configure<ConnectorOptions>(
    builder.Configuration.GetSection("Connector"));
// Worker
builder.Services.AddHostedService<Worker>();

var app = builder.Build();



// Şimdilik test endpointi
app.MapGet("/", () => "Connector is running.");
app.MapPost("/webhook",
    async (WebhookRequest request, IEnumerable<ISourceAdapter> adapters) =>
    {
        var webhookAdapter = adapters.OfType<WebhookAdapter>().FirstOrDefault();

        if (webhookAdapter is null)
            return Results.Ok(); // 404 yerine sessizce geç

        await webhookAdapter.ReceiveAsync(request);

        return Results.Ok();
    });
app.Run();