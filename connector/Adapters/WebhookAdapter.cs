using connector.Contracts;
using connector.Models;

namespace connector.Adapters;

public class WebhookAdapter : ISourceAdapter
{
    public string Name => "webhook";

    public event Func<RawMessage, Task>? OnRawMessage;

    public Task ConnectAsync(CancellationToken cancellationToken)
    {
        // Webhook pasif bir adaptördür.
        // Dışarıdan HTTP isteği beklediği için burada yapılacak bir iş yok.
        return Task.CompletedTask;
    }

    public Task DisconnectAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task ReceiveAsync(RawMessage message)
    {
        if (OnRawMessage != null)
        {
            await OnRawMessage.Invoke(message);
        }
    }
}