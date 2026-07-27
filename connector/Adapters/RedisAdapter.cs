using connector.Contracts;
using connector.Models;
using connector.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace connector.Adapters;
public class RedisAdapter : ISourceAdapter
{
    public string Name => "Redis";

    public event Func<RawMessage, Task>? OnRawMessage;

    public async Task ConnectAsync(CancellationToken cancellationToken)
{
    _logger.LogInformation(
        "Connecting to Redis: {ConnectionString}",
        _options.ConnectionString);

    _redis = await ConnectionMultiplexer.ConnectAsync(
        _options.ConnectionString);

    _subscriber = _redis.GetSubscriber();

try
{
    await _subscriber.SubscribeAsync(
        _options.Channel,
        async (channel, value) =>
        {
            _logger.LogInformation(
                "Redis message received: {Message}",
                value.ToString());

            if (OnRawMessage is not null)
            {
                await OnRawMessage(new RawMessage
                {
                    Source = "Redis",
                    Payload = value.ToString(),
                    ReceivedAt = DateTime.UtcNow
                });
            }
        });

    _logger.LogInformation(
        "Subscribed to Redis channel: {Channel}",
        _options.Channel);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Redis subscribe failed.");
} }
public async Task DisconnectAsync(CancellationToken cancellationToken)
{
    if (_redis is not null)
    {
        await _redis.CloseAsync();
        await _redis.DisposeAsync();
    }

    _logger.LogInformation("Redis disconnected.");
}
    private readonly RedisOptions _options;
    private readonly ILogger<RedisAdapter> _logger;

    private ConnectionMultiplexer? _redis;
    private ISubscriber? _subscriber;

public RedisAdapter(
    IOptions<RedisOptions> options,
    ILogger<RedisAdapter> logger)
{
    _options = options.Value;
    _logger = logger;
}



}