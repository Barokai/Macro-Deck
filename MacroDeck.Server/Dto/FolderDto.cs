namespace MacroDeck.Server.Dto;

public class FolderDto
{
    public string FolderId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsRootFolder { get; set; }
    public List<string> ChildFolderIds { get; set; } = new();
    public int ButtonCount { get; set; }
    public string ApplicationToTrigger { get; set; } = string.Empty;
}
