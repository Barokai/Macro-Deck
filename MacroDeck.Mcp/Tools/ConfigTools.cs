using System.ComponentModel;
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
        [Description("JSON object with only the fields to update, e.g. {\"blockNewConnections\":true}.")] string settingsJson) =>
        await _api.PatchJsonAsync("api/config", settingsJson);
}
