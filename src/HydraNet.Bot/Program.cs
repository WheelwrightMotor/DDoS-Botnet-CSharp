namespace HydraNet;

using HydraNet.Core;
using HydraNet.Config;

class Program
{
    static async Task Main(string[] args)
    {
        var config = BotConfig.LoadFromEnvironment();
        var client = new BotClient(config);

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            client.Shutdown();
        };

        await client.ConnectAsync();
    }
}
