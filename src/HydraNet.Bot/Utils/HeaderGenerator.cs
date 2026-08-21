namespace HydraNet.Utils;

public sealed class HeaderGenerator
{
    private static readonly string[] UserAgents =
    [
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 14_5) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.5 Safari/605.1.15",
        "Mozilla/5.0 (X11; Linux x86_64; rv:128.0) Gecko/20100101 Firefox/128.0",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36 Edg/126.0.0.0",
        "Mozilla/5.0 (iPhone; CPU iPhone OS 17_5 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.5 Mobile/15E148 Safari/604.1"
    ];

    private static readonly string[] AcceptLanguages =
    [
        "en-US,en;q=0.9",
        "en-GB,en;q=0.8",
        "de-DE,de;q=0.9,en;q=0.8",
        "fr-FR,fr;q=0.9,en;q=0.7",
        "ja-JP,ja;q=0.9,en;q=0.8"
    ];

    public Dictionary<string, string> Generate()
    {
        return new Dictionary<string, string>
        {
            ["User-Agent"] = UserAgents[Random.Shared.Next(UserAgents.Length)],
            ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
            ["Accept-Language"] = AcceptLanguages[Random.Shared.Next(AcceptLanguages.Length)],
            ["Accept-Encoding"] = "gzip, deflate, br",
            ["Connection"] = "keep-alive",
            ["Cache-Control"] = "no-cache"
        };
    }
}
