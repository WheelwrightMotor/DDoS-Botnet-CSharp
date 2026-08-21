namespace HydraNet.Core;

using System.Collections.Concurrent;
using HydraNet.Models;

public sealed class AttackScheduler
{
    private readonly ConcurrentQueue<AttackTask> _queue = new();
    private readonly List<Task> _activeTasks = [];
    private CancellationTokenSource _cts = new();

    public int QueuedCount => _queue.Count;
    public int ActiveCount => _activeTasks.Count(t => !t.IsCompleted);

    public void Enqueue(AttackTask task)
    {
        _queue.Enqueue(task);
        ProcessNext();
    }

    public void CancelAll()
    {
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        _activeTasks.Clear();
    }

    private void ProcessNext()
    {
        if (_queue.TryDequeue(out var task))
        {
            var execution = Task.Run(async () =>
            {
                if (task.DelaySeconds > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(task.DelaySeconds), _cts.Token);
                }
                await ExecuteAttackAsync(task, _cts.Token);
            }, _cts.Token);

            _activeTasks.Add(execution);
        }
    }

    private static Task ExecuteAttackAsync(AttackTask task, CancellationToken ct)
    {
        return Task.Delay(TimeSpan.FromSeconds(task.DurationSeconds), ct);
    }
}
