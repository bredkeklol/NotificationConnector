using System.Net.Http.Json;
using connector.Contracts;
using connector.Models;
using Microsoft.Extensions.Logging;

namespace connector.Services;

public class BackendSenderService : IBackendSender
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BackendSenderService> _logger;

    public BackendSenderService(
        HttpClient httpClient,
        ILogger<BackendSenderService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task SendAsync(
        NotificationEnvelope notification,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/api/notifications",
                notification,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "Notification successfully sent to backend.");
            }
            else
            {
                _logger.LogError(
                    "Backend returned {StatusCode}",
                    response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send notification to backend.");
        }
    }
}