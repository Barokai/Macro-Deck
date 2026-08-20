using MacroDeck.Server.Dto;
using MacroDeck.Server.Dto.Requests;
using MacroDeck.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace MacroDeck.Server.Controllers.Admin;

[ApiController]
[Route("api/devices")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceAdminService _devices;

    public DevicesController(IDeviceAdminService devices)
    {
        _devices = devices;
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<DeviceDto>> GetDevices() =>
        Ok(_devices.ListDevices());

    [HttpGet("{clientId}")]
    public ActionResult<DeviceDto> GetDevice(string clientId)
    {
        var device = _devices.GetDevice(clientId);
        return device is null ? NotFound() : Ok(device);
    }

    [HttpPut("{clientId}/profile")]
    public IActionResult AssignProfile(string clientId, [FromBody] AssignProfileRequest request)
    {
        return _devices.AssignProfile(clientId, request.ProfileId)
            ? NoContent()
            : NotFound("Device or profile not found.");
    }

    [HttpPut("{clientId}/blocked")]
    public IActionResult SetBlocked(string clientId, [FromQuery] bool blocked)
    {
        return _devices.SetBlocked(clientId, blocked) ? NoContent() : NotFound();
    }
}
