using System.Net.Http;
using System.Net.Http.Json;
using MacroDeck.Server.Dto;
using MacroDeck.Server.Services;
using SuchByte.MacroDeck.ExtensionStore;
using SuchByte.MacroDeck.Plugins;

namespace SuchByte.MacroDeck.Server.AdminServices;

public class PluginAdminService : IPluginAdminService
{
    private static readonly HttpClient _http = new();

    public IReadOnlyList<PluginDto> ListPlugins() =>
        PluginManager.Plugins.Values.Select(ToDto).ToList();

    public PluginDto? GetPlugin(string pluginName)
    {
        if (!PluginManager.Plugins.TryGetValue(pluginName, out var plugin)) return null;
        return ToDto(plugin);
    }

    public async Task<bool> InstallPluginAsync(string packageId)
    {
        try
        {
            // Queue the install via ExtensionStoreHelper on the background task
            await Task.Run(() => ExtensionStoreHelper.InstallPluginById(packageId));
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> SearchExtensionStoreAsync(string query, string type = "Plugin")
    {
        var url = $"{Constants.ExtensionStoreApiBaseUrl}/v2/extensions?query={Uri.EscapeDataString(query)}&type={Uri.EscapeDataString(type)}";
        try
        {
            var response = await _http.GetStringAsync(url);
            return response;
        }
        catch
        {
            return "[]";
        }
    }

    private static PluginDto ToDto(MacroDeckPlugin p) => new()
    {
        Name = p.Name,
        Author = p.Author ?? string.Empty,
        Version = p.Version ?? string.Empty,
        CanConfigure = p.CanConfigure,
        IsProtected = PluginManager.ProtectedPlugins.Contains(p),
        Actions = p.Actions.Select(a => new PluginActionInfoDto
        {
            Name = a.Name,
            Description = a.Description,
            CanConfigure = a.CanConfigure,
            ActionClass = a.GetType().Name,
        }).ToList(),
    };
}
