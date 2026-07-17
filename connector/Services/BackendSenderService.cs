using System.Net.Http.Json;
using connector.Models;

namespace connector.Services;

public class BackendSenderService
{
    private readonly HttpClient _httpClient;

    public BackendSenderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendAsync(NotificationEnvelope notification)
    {
        await _httpClient.PostAsJsonAsync(
            "http://backend:8080/api/notifications",
            notification);
    }
}