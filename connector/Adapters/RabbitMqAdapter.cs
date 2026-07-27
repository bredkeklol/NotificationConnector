using connector.Contracts;
using connector.Models;
using connector.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace connector.Adapters;
using System.Text;
public class RabbitMqAdapter : ISourceAdapter
{
    private readonly ILogger<RabbitMqAdapter> _logger;
    private readonly RabbitMqOptions _options;

    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqAdapter(
        ILogger<RabbitMqAdapter> logger,
        IOptions<RabbitMqOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public string Name => "rabbitmq";

    public event Func<RawMessage, Task>? OnRawMessage;

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Connecting to RabbitMQ: {Host}:{Port}",
            _options.HostName,
            _options.Port);

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = "/",
            AutomaticRecoveryEnabled = true
        };

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _connection = factory.CreateConnection();

                _logger.LogInformation("RabbitMQ connection established.");

                _channel = _connection.CreateModel();

                _logger.LogInformation("RabbitMQ channel created.");

                _channel.QueueDeclare(
                    queue: _options.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _logger.LogInformation(
                    "RabbitMQ queue '{Queue}' declared.",
                    _options.QueueName);
var consumer = new EventingBasicConsumer(_channel); 
                
        consumer.Received += async (_, eventArgs) =>
{
    var body = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

    _logger.LogInformation(
        "RabbitMQ message received: {Message}",
        body);

    var rawMessage = new RawMessage
    {
        Source = "RabbitMQ",
        Payload = body,
        ReceivedAt = DateTime.UtcNow
    };

    if (OnRawMessage is not null)
    {
        await OnRawMessage(rawMessage);
    }
};
   _channel.BasicConsume(
    queue: _options.QueueName,
    autoAck: true,
    consumer: consumer);

_logger.LogInformation(
    "RabbitMQ consumer started for queue '{Queue}'.",
    _options.QueueName);             
                
                
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "RabbitMQ is not ready. Retrying in 5 seconds...");

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }

    public Task DisconnectAsync(CancellationToken cancellationToken)
    {
        _channel?.Dispose();
        _connection?.Dispose();

        _logger.LogInformation("RabbitMQ Adapter stopped.");

        return Task.CompletedTask;
    }
}