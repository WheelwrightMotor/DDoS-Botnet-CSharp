namespace HydraNet.Network;

using System.Net.Sockets;

public sealed class RawSocket : IDisposable
{
    private Socket? _socket;
    private readonly ProtocolType _protocol;

    public bool IsOpen => _socket is not null;

    public RawSocket(ProtocolType protocol)
    {
        _protocol = protocol;
    }

    public void Open()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, _protocol);
        _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
    }

    public int Send(byte[] data, System.Net.EndPoint target)
    {
        if (_socket is null)
            throw new InvalidOperationException("Socket not open");

        return _socket.SendTo(data, target);
    }

    public void SetTtl(short ttl)
    {
        _socket?.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, ttl);
    }

    public void SetNoDelay(bool noDelay)
    {
        _socket?.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, noDelay);
    }

    public void Dispose()
    {
        _socket?.Close();
        _socket?.Dispose();
        _socket = null;
    }
}
