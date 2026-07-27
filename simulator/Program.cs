using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var webhookUrl = configuration["Connector:WebhookUrl"]!;
var httpClient = new HttpClient();

int counter = 1;

var sources = new[]
{
    "Simulator",
    "RabbitMQ",
    "Redis",
    "Webhook",
    "WebSocket"
};

var random = new Random();

while (true)
{
    var notification = new
    {
        Id = Guid.NewGuid(),
        Source = sources[random.Next(sources.Length)],
        Title = $"Notification {counter}",
        Message = $"This is notification number {counter}",
        Timestamp = DateTime.UtcNow
    };

    try
{
    var response = await httpClient.PostAsJsonAsync(
    webhookUrl,
    notification);

    Console.WriteLine(
        $"Sent Notification {counter} - Status: {response.StatusCode}");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

    counter++;

    await Task.Delay(5000);
}