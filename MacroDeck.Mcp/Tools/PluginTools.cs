using System.ComponentModel;
using MacroDeck.Mcp;
using ModelContextProtocol.Server;

/// <summary>MCP tools for browsing and installing MacroDeck plugins.</summary>
[McpServerToolType]
public class PluginTools
{
    private readonly MacroDeckApiClient _api;
    public PluginTools(MacroDeckApiClient api) => _api = api;

    [McpServerTool, Description(
        "List all installed MacroDeck plugins. Returns plugin name, author, version, whether it can be configured, " +
        "and – most importantly – the list of available action classes with their names and descriptions. " +
        "Use this to discover what pluginName and actionClass values to use when creating buttons.")]
    public async Task<string> ListPlugins() =>
        await _api.GetJsonAsync("api/plugins");

    [McpServerTool, Description(
        "Get details for a single installed plugin, including all its available action classes.")]
    public async Task<string> GetPlugin(
        [Description("The exact plugin name as returned by list_plugins.")] string pluginName) =>
        await _api.GetJsonAsync($"api/plugins/{Uri.EscapeDataString(pluginName)}");

    [McpServerTool, Description(
        "Search the MacroDeck Extension Store for plugins or icon packs by keyword. " +
        "Returns a list of available packages with their packageId, name, author, and description. " +
        "After finding a desired plugin, use install_plugin to install it.")]
    public async Task<string> SearchExtensionStore(
        [Description("Search term (e.g. 'obs studio', 'spotify', 'volume').")] string query,
        [Description("Type to search: 'Plugin' or 'IconPack'. Default is 'Plugin'.")] string type = "Plugin") =>
        await _api.GetJsonAsync($"api/plugins/store/search?q={Uri.EscapeDataString(query)}&type={Uri.EscapeDataString(type)}");

    [McpServerTool, Description(
        "Install a MacroDeck plugin from the Extension Store by its packageId. " +
        "The plugin will be downloaded and staged; MacroDeck must be restarted for it to become active.")]
    public async Task<string> InstallPlugin(
        [Description("The packageId of the plugin (from search_extension_store results).")] string packageId) =>
        await _api.PostJsonAsync("api/plugins/store/install", new { packageId });
}
