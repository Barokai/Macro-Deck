using MacroDeck.Server.Dto;
using MacroDeck.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace MacroDeck.Server.Controllers.Admin;

[ApiController]
[Route("api/icons")]
public class IconsController : ControllerBase
{
    private readonly IIconAdminService _icons;

    public IconsController(IIconAdminService icons)
    {
        _icons = icons;
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<IconPackDto>> GetIconPacks() =>
        Ok(_icons.ListIconPacks());

    [HttpGet("{iconPackName}")]
    public ActionResult<IReadOnlyList<IconDto>> GetIcons(string iconPackName)
    {
        var icons = _icons.ListIcons(iconPackName);
        return icons.Count == 0 && !_icons.ListIconPacks().Any(p => p.Name == iconPackName)
            ? NotFound()
            : Ok(icons);
    }
}
