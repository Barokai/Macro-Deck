using MacroDeck.Server.Dto;
using MacroDeck.Server.Services;
using SuchByte.MacroDeck.Icons;

namespace SuchByte.MacroDeck.Server.AdminServices;

public class IconAdminService : IIconAdminService
{
    public IReadOnlyList<IconPackDto> ListIconPacks() =>
        IconManager.IconPacks
            .Select(p => new IconPackDto
            {
                Name = p.Name,
                PackageId = p.PackageId ?? string.Empty,
                Author = p.Author ?? string.Empty,
                Version = p.Version ?? string.Empty,
                IconCount = p.Icons?.Count ?? 0,
            })
            .ToList();

    public IReadOnlyList<IconDto> ListIcons(string iconPackName)
    {
        var pack = IconManager.IconPacks.Find(p => p.Name == iconPackName);
        if (pack?.Icons is null) return [];

        return pack.Icons
            .Select(icon => new IconDto
            {
                IconId = icon.IconId,
                IconPackName = pack.Name,
                IconString = $"{pack.Name}.{icon.IconId}",
            })
            .ToList();
    }
}
