using MacroDeck.Server.Dto;
using MacroDeck.Server.Dto.Requests;

namespace MacroDeck.Server.Services;

public interface IDeviceAdminService
{
    IReadOnlyList<DeviceDto> ListDevices();
    DeviceDto? GetDevice(string clientId);
    bool AssignProfile(string clientId, string profileId);
    bool SetBlocked(string clientId, bool blocked);
}
