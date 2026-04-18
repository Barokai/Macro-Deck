namespace MacroDeck.Server.Dto;

public class IconPackDto
{
    public string Name { get; set; } = string.Empty;
    public string PackageId { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public int IconCount { get; set; }
}

public class IconDto
{
    public string IconId { get; set; } = string.Empty;
    public string IconPackName { get; set; } = string.Empty;
    /// <summary>Convenience combined string usable directly in update_button iconOff/iconOn fields.</summary>
    public string IconString { get; set; } = string.Empty;
}
