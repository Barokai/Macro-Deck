using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MacroDeck.Mcp;

/// <summary>
/// Typed HTTP client for the MacroDeck admin REST API.
/// </summary>
public class MacroDeckApiClient
{
    private readonly HttpClient _http;

    public MacroDeckApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<string> GetJsonAsync(string path)
    {
        var response = await _http.GetAsync(path);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostJsonAsync(string path, object body)
    {
        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _http.PostAsync(path, content);
        var responseText = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"POST {path} failed ({(int)response.StatusCode}): {responseText}");
        return responseText;
    }

    public async Task<string> PutJsonAsync(string path, object body)
    {
        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _http.PutAsync(path, content);
        var responseText = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"PUT {path} failed ({(int)response.StatusCode}): {responseText}");
        return responseText;
    }

    public async Task<string> PatchJsonAsync(string path, object body)
    {
        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Patch, path) { Content = content };
        var response = await _http.SendAsync(request);
        var responseText = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"PATCH {path} failed ({(int)response.StatusCode}): {responseText}");
        return responseText;
    }

    public async Task<bool> DeleteAsync(string path)
    {
        var response = await _http.DeleteAsync(path);
        return response.IsSuccessStatusCode;
    }
}
