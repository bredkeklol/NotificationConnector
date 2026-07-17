using connector.Contracts;
using connector.Models;

namespace connector.Adapters;

public class FakeAdapter : ISourceAdapter
{
    private Timer? _timer;

    public string Name => "fake";

    public event Func<RawMessage, Task>? OnRawMessage;

    public Task ConnectAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(async _ =>
        {
            if (OnRawMessage != null)
            {
                await OnRawMessage.Invoke(new RawMessage
                {
                    Source = "FakeAdapter",
                    Payload = $"Test message {DateTime.Now:HH:mm:ss}",
                    ReceivedAt = DateTime.UtcNow
                });
            }
        },
        null,
        TimeSpan.Zero,
        TimeSpan.FromSeconds(5));

        return Task.CompletedTask;
    }

    public Task DisconnectAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        return Task.CompletedTask;
    }
}