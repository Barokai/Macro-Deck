namespace MacroDeck.Server.Dto;

public class PluginActionInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool CanConfigure { get; set; }
    public string ActionClass { get; set; } = string.Empty;
}

public class PluginDto
{
    public string Name { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public bool CanConfigure { get; set; }
    public bool IsProtected { get; set; }
    public List<PluginActionInfoDto> Actions { get; set; } = new();
}
