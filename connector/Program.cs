using connector;
using connector.Adapters;
using connector.Contracts;
using connector.Core;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ISourceAdapter, FakeAdapter>();

builder.Services.AddSingleton<ISourceAdapter, WebhookAdapter>();

builder.Services.AddSingleton<IConnector, ConnectorCore>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();