using MacroDeck.Server.Dto;
using MacroDeck.Server.Dto.Requests;
using MacroDeck.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace MacroDeck.Server.Controllers.Admin;

[ApiController]
[Route("api/plugins")]
public class PluginsController : ControllerBase
{
    private readonly IPluginAdminService _plugins;

    public PluginsController(IPluginAdminService plugins)
    {
        _plugins = plugins;
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<PluginDto>> GetPlugins() =>
        Ok(_plugins.ListPlugins());

    [HttpGet("{pluginName}")]
    public ActionResult<PluginDto> GetPlugin(string pluginName)
    {
        var plugin = _plugins.GetPlugin(pluginName);
        return plugin is null ? NotFound() : Ok(plugin);
    }

    [HttpGet("store/search")]
    public async Task<ActionResult<string>> SearchStore([FromQuery] string q, [FromQuery] string type = "Plugin")
    {
        var results = await _plugins.SearchExtensionStoreAsync(q, type);
        return Content(results, "application/json");
    }

    [HttpPost("store/install")]
    public async Task<IActionResult> InstallPlugin([FromBody] InstallExtensionRequest request)
    {
        var success = await _plugins.InstallPluginAsync(request.PackageId);
        return success ? Ok(new { message = "Plugin queued for install. Restart MacroDeck to apply." })
                       : BadRequest(new { error = "Install failed. Check the package ID." });
    }
}
