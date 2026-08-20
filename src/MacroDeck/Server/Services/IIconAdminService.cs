using MacroDeck.Server.Dto;

namespace MacroDeck.Server.Services;

public interface IIconAdminService
{
    IReadOnlyList<IconPackDto> ListIconPacks();
    IReadOnlyList<IconDto> ListIcons(string iconPackName);
}
