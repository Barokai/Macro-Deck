using System.ComponentModel;
using MacroDeck.Mcp;
using ModelContextProtocol.Server;

/// <summary>MCP tools for managing connected MacroDeck devices.</summary>
[McpServerToolType]
public class DeviceTools
{
    private readonly MacroDeckApiClient _api;
    public DeviceTools(MacroDeckApiClient api) => _api = api;

    [McpServerTool, Description(
        "List all known MacroDeck devices (phones, tablets, web clients). " +
        "Returns clientId, display name, assigned profile, device type, and whether the device is currently online.")]
    public async Task<string> ListDevices() =>
        await _api.GetJsonAsync("api/devices");

    [McpServerTool, Description("Assign a different profile to a device. The change takes effect immediately for connected devices.")]
    public async Task<string> AssignProfileToDevice(
        [Description("The clientId of the device (from list_devices).")] string clientId,
        [Description("The profileId to assign (from list_profiles).")] string profileId) =>
        await _api.PutJsonAsync($"api/devices/{Uri.EscapeDataString(clientId)}/profile", new { profileId });

    [McpServerTool, Description("Block or unblock a device. Blocked devices cannot connect to MacroDeck.")]
    public async Task<string> SetDeviceBlocked(
        [Description("The clientId of the device.")] string clientId,
        [Description("true to block, false to unblock.")] bool blocked)
    {
        var result = await _api.PutJsonAsync(
            $"api/devices/{Uri.EscapeDataString(clientId)}/blocked?blocked={blocked.ToString().ToLowerInvariant()}",
            new { });
        return result;
    }
}
