using connector.Models;

namespace connector.Contracts;

public interface ISourceAdapter
{
    string Name { get; }

    Task ConnectAsync(CancellationToken cancellationToken);

    Task DisconnectAsync(CancellationToken cancellationToken);

    event Func<RawMessage, Task>? OnRawMessage;}