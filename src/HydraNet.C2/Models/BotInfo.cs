namespace HydraNet.C2.Models;

public sealed record BotInfo
{
    public required string BotId { get; init; }
    public required string OperatingSystem { get; init; }
    public required string MachineName { get; init; }
    public required int Cores { get; init; }
    public required DateTime RegisteredAt { get; init; }
    public required DateTime LastSeen { get; init; }
    public required bool IsOnline { get; init; }
    public string Country { get; init; } = "Unknown";
}
