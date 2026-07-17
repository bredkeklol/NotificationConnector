using connector.Contracts;
using connector.Models;

namespace connector.Core;

public class ConnectorCore : IConnector
{
    private readonly List<ISourceAdapter> _adapters = new();

    public event Func<NotificationEnvelope, Task>? OnMessage;
    private readonly IBackendSender _backendSender;
    public ConnectorCore(
    IEnumerable<ISourceAdapter> adapters,
    IBackendSender backendSender)
{
    _backendSender = backendSender;

    foreach (var adapter in adapters)
    {
        Register(adapter);
    }
}
    
    public void Register(ISourceAdapter adapter)
    {
        if (_adapters.Any(a => a.Name == adapter.Name))
            return;

        adapter.OnRawMessage += HandleRawMessageAsync;
        _adapters.Add(adapter);
    }

    public void Unregister(string adapterName)
    {
        var adapter = _adapters.FirstOrDefault(a => a.Name == adapterName);

        if (adapter == null)
            return;

        adapter.OnRawMessage -= HandleRawMessageAsync;
        _adapters.Remove(adapter);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var adapter in _adapters)
        {
            await adapter.ConnectAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var adapter in _adapters)
        {
            await adapter.DisconnectAsync(cancellationToken);
        }
    }

   private async Task HandleRawMessageAsync(RawMessage rawMessage)
{
    var notification = new NotificationEnvelope
    {
        Source = rawMessage.Source,
        Title = "Incoming Notification",
        Message = rawMessage.Payload,
        Timestamp = rawMessage.ReceivedAt
    };

    await _backendSender.SendAsync(notification, CancellationToken.None);

if (OnMessage != null)
{
    await OnMessage.Invoke(notification);
}
}
}