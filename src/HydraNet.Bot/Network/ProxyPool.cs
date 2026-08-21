namespace HydraNet.Network;

public sealed class ProxyPool
{
    private readonly List<string> _proxies = [];
    private int _index;

    public int Count => _proxies.Count;

    public void LoadFromFile(string path)
    {
        if (!File.Exists(path)) return;
        var lines = File.ReadAllLines(path)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();
        _proxies.AddRange(lines);
    }

    public void AddRange(IEnumerable<string> proxies)
    {
        _proxies.AddRange(proxies);
    }

    public string? GetNext()
    {
        if (_proxies.Count == 0) return null;
        var idx = Interlocked.Increment(ref _index) % _proxies.Count;
        return _proxies[idx];
    }

    public string? GetRandom()
    {
        if (_proxies.Count == 0) return null;
        return _proxies[Random.Shared.Next(_proxies.Count)];
    }

    public void Remove(string proxy)
    {
        _proxies.Remove(proxy);
    }

    public void Clear()
    {
        _proxies.Clear();
        _index = 0;
    }
}
