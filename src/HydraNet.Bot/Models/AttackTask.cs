namespace HydraNet.Models;

public sealed record AttackTask
{
    public required string TaskId { get; init; }
    public required string Method { get; init; }
    public required string Target { get; init; }
    public int Port { get; init; } = 80;
    public int DurationSeconds { get; init; } = 60;
    public int Threads { get; init; } = 10;
    public int DelaySeconds { get; init; } = 0;
    public Dictionary<string, string> Options { get; init; } = new();
}
