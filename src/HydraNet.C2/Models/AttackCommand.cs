namespace HydraNet.C2.Models;

public sealed record AttackCommand
{
    public required string Method { get; init; }
    public required string Target { get; init; }
    public int Port { get; init; } = 80;
    public int DurationSeconds { get; init; } = 60;
    public int Threads { get; init; } = 10;
    public string[] BotIds { get; init; } = [];
    public Dictionary<string, string> Options { get; init; } = new();
}
