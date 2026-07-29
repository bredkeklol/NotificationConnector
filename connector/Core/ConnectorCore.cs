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
        Console.WriteLine($"Registering adapter: {adapter.Name}");
        Register(adapter);
    }
}
    
   public void Register(ISourceAdapter adapter)
{
    if (_adapters.Any(a => a.Name == adapter.Name))
        return;

    adapter.OnRawMessage += async raw =>
    {
        try
        {
            await HandleRawMessageAsync(raw);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Failed to process message from adapter {adapter.Name}: {ex.Message}");
        }
    };

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
    var tasks = _adapters.Select(adapter =>
        adapter.ConnectAsync(cancellationToken));

    await Task.WhenAll(tasks);
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
