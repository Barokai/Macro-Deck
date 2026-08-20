namespace MacroDeck.Server.Dto;

public class ActionDto
{
    public string PluginName { get; set; } = string.Empty;
    public string ActionClass { get; set; } = string.Empty;
    public string Configuration { get; set; } = string.Empty;
    public string ConfigurationSummary { get; set; } = string.Empty;
}

public class ButtonDto
{
    public string Guid { get; set; } = string.Empty;
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public bool State { get; set; }
    public List<ActionDto> Actions { get; set; } = new();
    public List<ActionDto> ActionsRelease { get; set; } = new();
    public List<ActionDto> ActionsLongPress { get; set; } = new();
    public List<ActionDto> ActionsLongPressRelease { get; set; } = new();
    public string? LabelOffText { get; set; }
    public string? LabelOnText { get; set; }
    public string? StateBindingVariable { get; set; }
    public string? IconPack { get; set; }
    public string? IconName { get; set; }
    public string? IconNameOn { get; set; }
    public string? IconOff { get; set; }
    public string? IconOn { get; set; }
    public string? BackgroundColorOff { get; set; }
    public string? BackgroundColorOn { get; set; }
    public string? LabelColorOff { get; set; }
    public string? LabelColorOn { get; set; }
}
