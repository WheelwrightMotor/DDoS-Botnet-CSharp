namespace HydraNet.C2.Utils;

using System.Net;
using System.Text.Json;

public sealed class GeoLocator
{
    private readonly HttpClient _client;
    private readonly Dictionary<string, string> _cache = new();

    public GeoLocator()
    {
        _client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
    }

    public async Task<string> GetCountryAsync(string ipAddress)
    {
        if (_cache.TryGetValue(ipAddress, out var cached))
            return cached;

        try
        {
            var response = await _client.GetStringAsync($"http://ip-api.com/json/{ipAddress}?fields=country,countryCode");
            var data = JsonSerializer.Deserialize<GeoData>(response);
            var country = data?.CountryCode ?? "XX";
            _cache[ipAddress] = country;
            return country;
        }
        catch
        {
            return "XX";
        }
    }

    public string GetCountryFromIp(IPAddress address)
    {
        var bytes = address.GetAddressBytes();
        if (bytes.Length < 4) return "XX";
        return bytes[0] switch
        {
            >= 1 and <= 126 => "US",
            >= 128 and <= 191 => "EU",
            _ => "XX"
        };
    }

    private sealed record GeoData
    {
        public string? Country { get; init; }
        public string? CountryCode { get; init; }
    }
}
