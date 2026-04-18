using System.ComponentModel;
using MacroDeck.Mcp;
using ModelContextProtocol.Server;

/// <summary>MCP tools for browsing installed MacroDeck icon packs and their icons.</summary>
[McpServerToolType]
public class IconTools
{
    private readonly MacroDeckApiClient _api;
    public IconTools(MacroDeckApiClient api) => _api = api;

    [McpServerTool, Description(
        "List all installed icon packs. Returns each pack's name, author, version, and icon count. " +
        "Use the pack 'name' value with list_icons to discover valid icon IDs. " +
        "IMPORTANT: Always call list_icons before assigning an icon to a button — icon IDs must match exactly.")]
    public async Task<string> ListIconPacks() =>
        await _api.GetJsonAsync("api/icons");

    [McpServerTool, Description(
        "List all icons in a specific icon pack. Returns each icon's 'iconId' and the ready-to-use 'iconString' " +
        "(format: 'PackName.iconId') to supply as iconOff or iconOn in update_button. " +
        "Always use this to verify icon IDs before assigning — guessed names will silently show nothing.")]
    public async Task<string> ListIcons(
        [Description("The exact icon pack name as returned by list_icon_packs (e.g. 'Fluent').")] string iconPackName) =>
        await _api.GetJsonAsync($"api/icons/{Uri.EscapeDataString(iconPackName)}");
}
