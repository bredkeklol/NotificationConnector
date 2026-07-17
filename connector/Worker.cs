using connector.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace connector;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConnector _connector;

    public Worker(ILogger<Worker> logger, IConnector connector)
{
    _logger = logger;
    _connector = connector;

    _connector.OnMessage += async notification =>
    {
        _logger.LogInformation(
            "Notification received: {Source} - {Message}",
            notification.Source,
            notification.Message);

        await Task.CompletedTask;
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