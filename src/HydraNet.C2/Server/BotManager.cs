namespace HydraNet.C2.Server;

using System.Collections.Concurrent;
using HydraNet.C2.Models;

public sealed class BotManager
{
    private readonly ConcurrentDictionary<string, BotInfo> _bots = new();

    public int OnlineCount => _bots.Values.Count(b => b.IsOnline);
    public int TotalCount => _bots.Count;

    public void Register(string botId, Dictionary<string, string> info)
    {
        var bot = new BotInfo
        {
            BotId = botId,
            OperatingSystem = info.GetValueOrDefault("os", "Unknown"),
            MachineName = info.GetValueOrDefault("machine", "Unknown"),
            Cores = int.TryParse(info.GetValueOrDefault("cores", "0"), out var c) ? c : 0,
            RegisteredAt = DateTime.UtcNow,
            LastSeen = DateTime.UtcNow,
            IsOnline = true
        };
        _bots[botId] = bot;
    }

    public void UpdateLastSeen(string botId)
    {
        if (_bots.TryGetValue(botId, out var bot))
        {
            _bots[botId] = bot with { LastSeen = DateTime.UtcNow, IsOnline = true };
        }
    }

    public BotInfo? GetBot(string botId)
    {
        _bots.TryGetValue(botId, out var bot);
        return bot;
    }

    public IEnumerable<BotInfo> GetAllBots() => _bots.Values;

    public IEnumerable<string> GetOnlineBotIds()
    {
        return _bots.Values
            .Where(b => b.IsOnline && DateTime.UtcNow - b.LastSeen < TimeSpan.FromMinutes(5))
            .Select(b => b.BotId);
    }

    public void MarkOffline(string botId)
    {
        if (_bots.TryGetValue(botId, out var bot))
        {
            _bots[botId] = bot with { IsOnline = false };
        }
    }
}
