namespace MacroDeck.Server.Dto;

public class ProfileDto
{
    public string ProfileId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int Rows { get; set; }
    public int Columns { get; set; }
    public int ButtonSpacing { get; set; }
    public int ButtonRadius { get; set; }
    public bool ButtonBackground { get; set; }
    public string ProfileTarget { get; set; } = string.Empty;
    public int FolderCount { get; set; }
}
