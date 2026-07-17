using connector.Models;

namespace connector.Contracts;

public interface IBackendSender
{
    Task SendAsync(NotificationEnvelope notification, CancellationToken cancellationToken);
}