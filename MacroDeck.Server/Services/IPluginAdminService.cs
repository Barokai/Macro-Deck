using MacroDeck.Server.Dto;

namespace MacroDeck.Server.Services;

public interface IPluginAdminService
{
    IReadOnlyList<PluginDto> ListPlugins();
    PluginDto? GetPlugin(string pluginName);

    /// <summary>Installs a plugin package from the Extension Store by packageId.</summary>
    Task<bool> InstallPluginAsync(string packageId);

    /// <summary>Searches the Extension Store. Returns JSON string list of results.</summary>
    Task<string> SearchExtensionStoreAsync(string query, string type = "Plugin");
}
