using System.ComponentModel;
using System.Text.Json;
using MacroDeck.Mcp;
using ModelContextProtocol.Server;

/// <summary>MCP tools for reading and updating MacroDeck server configuration.</summary>
[McpServerToolType]
public class ConfigTools
{
    private readonly MacroDeckApiClient _api;
    public ConfigTools(MacroDeckApiClient api) => _api = api;

    [McpServerTool, Description(
        "Get the current MacroDeck server configuration: port, SSL, auto-update settings, ADB, language, and connection policies.")]
    public async Task<string> GetConfiguration() =>
        await _api.GetJsonAsync("api/config");

    [McpServerTool, Description(
        "Update one or more MacroDeck configuration settings. Only provide the fields you want to change. " +
        "Supported fields: autoUpdates (bool), updateBetaVersions (bool), enableAdbServer (bool), " +
        "enableAdbAutoStartApp (bool), askOnNewConnections (bool), blockNewConnections (bool), language (string).")]
    public async Task<string> UpdateConfiguration(
        [Description("JSON object with only the fields to update, e.g. {\"blockNewConnections\":true}.")] string settingsJson)
    {
        try
        {
            // Parse the JSON string into a dictionary for flexibility
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var patch = JsonSerializer.Deserialize<Dictionary<string, object>>(settingsJson, options)
                ?? throw new ArgumentException("Invalid JSON in settingsJson parameter");
            
            return await _api.PatchJsonAsync("api/config", patch);
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"Invalid JSON in settingsJson: {ex.Message}", ex);
        }
    }
}
