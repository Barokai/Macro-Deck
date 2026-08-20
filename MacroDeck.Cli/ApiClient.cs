using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MacroDeck.Cli;

/// <summary>
/// Thin HTTP wrapper over the MacroDeck admin REST API.
/// URL and API key are resolved in priority order:
///   1. CLI options (--url / --key)
///   2. Environment variables MACRODECK_URL / MACRODECK_API_KEY
///   3. Defaults (http://localhost:8191)
/// </summary>
public class ApiClient
{
    private static readonly JsonSerializerOptions PrettyJson = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly HttpClient _http;

    public ApiClient(string baseUrl, string apiKey)
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/")
        };
        _http.DefaultRequestHeaders.Add("X-MacroDeck-Admin-Key", apiKey);
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public static ApiClient FromEnvironment(string? urlOverride = null, string? keyOverride = null)
    {
        var url = urlOverride
            ?? Environment.GetEnvironmentVariable("MACRODECK_URL")
            ?? "http://localhost:8191";
        var key = keyOverride
            ?? Environment.GetEnvironmentVariable("MACRODECK_API_KEY")
            ?? string.Empty;
        return new ApiClient(url, key);
    }

    public async Task<string> GetAsync(string path)
    {
        var response = await _http.GetAsync(path);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"GET {path} → {(int)response.StatusCode}: {body}");
        return PrettyFormat(body);
    }

    public async Task<string> PostAsync(string path, object body)
    {
        var response = await _http.PostAsync(path, JsonContent(body));
        var raw = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"POST {path} → {(int)response.StatusCode}: {raw}");
        return PrettyFormat(raw);
    }

    public async Task<string> PutAsync(string path, object body)
    {
        var response = await _http.PutAsync(path, JsonContent(body));
        var raw = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"PUT {path} → {(int)response.StatusCode}: {raw}");
        return PrettyFormat(raw);
    }

    public async Task<string> PatchAsync(string path, object body)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, path) { Content = JsonContent(body) };
        var response = await _http.SendAsync(request);
        var raw = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"PATCH {path} → {(int)response.StatusCode}: {raw}");
        return PrettyFormat(raw);
    }

    public async Task<bool> DeleteAsync(string path)
    {
        var response = await _http.DeleteAsync(path);
        return response.IsSuccessStatusCode;
    }

    private static StringContent JsonContent(object body)
    {
        var json = JsonSerializer.Serialize(body, PrettyJson);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static string PrettyFormat(string raw)
    {
        try
        {
            var doc = JsonDocument.Parse(raw);
            return JsonSerializer.Serialize(doc, PrettyJson);
        }
        catch
        {
            return raw;
        }
    }
}
