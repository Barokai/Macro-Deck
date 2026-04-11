using MacroDeck.Server.Dto;
using MacroDeck.Server.Dto.Requests;
using MacroDeck.Server.Services;
using SuchByte.MacroDeck.Device;
using SuchByte.MacroDeck.Profiles;

namespace SuchByte.MacroDeck.Server.AdminServices;

public class DeviceAdminService : IDeviceAdminService
{
    public IReadOnlyList<DeviceDto> ListDevices() =>
        DeviceManager.GetKnownDevices().Select(ToDto).ToList();

    public DeviceDto? GetDevice(string clientId)
    {
        var device = DeviceManager.GetMacroDeckDevice(clientId);
        return device is null ? null : ToDto(device);
    }

    public bool AssignProfile(string clientId, string profileId)
    {
        var device = DeviceManager.GetMacroDeckDevice(clientId);
        if (device is null) return false;
        var profile = ProfileManager.FindProfileById(profileId);
        if (profile is null) return false;
        DeviceManager.SetProfile(device, profile);
        return true;
    }

    public bool SetBlocked(string clientId, bool blocked)
    {
        var device = DeviceManager.GetMacroDeckDevice(clientId);
        if (device is null) return false;
        DeviceManager.SetBlocked(device, blocked);
        return true;
    }

    private static DeviceDto ToDto(MacroDeckDevice d) => new()
    {
        ClientId = d.ClientId,
        DisplayName = d.DisplayName,
        ProfileId = d.ProfileId ?? string.Empty,
        DeviceType = d.DeviceType.ToString(),
        Blocked = d.Blocked,
        Available = d.Available,
        Configuration = d.Configuration is null ? null : new DeviceConfigDto
        {
            Brightness = d.Configuration.Brightness,
            AutoConnect = d.Configuration.AutoConnect,
            WakeLockMethod = d.Configuration.WakeLockMethod.ToString(),
        },
    };
}
