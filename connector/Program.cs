using connector;
using connector.Adapters;
using connector.Contracts;
using connector.Core;
using connector.Services;
using connector.Models;
var builder = WebApplication.CreateBuilder(args);

// Adapter'lar
builder.Services.AddSingleton<ISourceAdapter, FakeAdapter>();
builder.Services.AddSingleton<ISourceAdapter, WebhookAdapter>();

// Core
builder.Services.AddSingleton<IConnector, ConnectorCore>();

// Backend Sender
builder.Services.AddHttpClient<IBackendSender, BackendSenderService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5033");
});

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
            return Results.NotFound("WebhookAdapter not found.");

        await webhookAdapter.ReceiveAsync(request);

        return Results.Ok();
    });
app.Run();