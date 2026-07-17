using Microsoft.Extensions.Hosting;

namespace websocketserver.Services;

public class NotificationPublisherWorker : BackgroundService
{
    private readonly NotificationPublisher _publisher;

    public NotificationPublisherWorker(NotificationPublisher publisher)
    {
        _publisher = publisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _publisher.PublishTestNotificationAsync();

            await Task.Delay(
                TimeSpan.FromSeconds(5),
                stoppingToken);
        }
    }
}