using connector.Models;

namespace connector.Contracts;

public interface IConnector
{
    void Register(ISourceAdapter adapter);

    void Unregister(string adapterName);

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    event Func<NotificationEnvelope, Task>? OnMessage;
}