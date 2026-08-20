using MacroDeck.Server.Dto;
using MacroDeck.Server.Dto.Requests;
using MacroDeck.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace MacroDeck.Server.Controllers.Admin;

[ApiController]
[Route("api/config")]
public class ConfigController : ControllerBase
{
    private readonly IConfigAdminService _config;

    public ConfigController(IConfigAdminService config)
    {
        _config = config;
    }

    [HttpGet]
    public ActionResult<ConfigDto> GetConfig() =>
        Ok(_config.GetConfig());

    [HttpPatch]
    public ActionResult<ConfigDto> UpdateConfig([FromBody] UpdateConfigRequest request) =>
        Ok(_config.UpdateConfig(request));
}
