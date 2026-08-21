namespace HydraNet.C2;

using HydraNet.C2.Server;

class Program
{
    static async Task Main(string[] args)
    {
        var port = args.Length > 0 && int.TryParse(args[0], out var p) ? p : 8080;
        var server = new C2Server(port);

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            server.Stop();
        };

        Console.WriteLine($"[*] C2 Server starting on port {port}");
        await server.StartAsync();
    }
}
