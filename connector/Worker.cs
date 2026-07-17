using connector.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace connector;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConnector _connector;
    private readonly IBackendSender _backendSender;
    public Worker(
    ILogger<Worker> logger,
    IConnector connector,
    IBackendSender backendSender)
{
    _logger = logger;
    _connector = connector;
    _backendSender = backendSender;

    _connector.OnMessage += async notification =>
    {
        await _backendSender.SendAsync(notification, CancellationToken.None);
    };
}

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Connector is starting...");

        await _connector.StartAsync(stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Connector is stopping...");

        await _connector.StopAsync(cancellationToken);

        await base.StopAsync(cancellationToken);
    }
}