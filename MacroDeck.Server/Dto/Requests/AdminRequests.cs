using System.ComponentModel.DataAnnotations;

namespace MacroDeck.Server.Dto.Requests;

public class CreateProfileRequest
{
    [Required, MinLength(1)]
    public string DisplayName { get; set; } = string.Empty;
    public int Rows { get; set; } = 3;
    public int Columns { get; set; } = 5;
    public int ButtonSpacing { get; set; } = 10;
    public int ButtonRadius { get; set; } = 40;
    public bool ButtonBackground { get; set; } = true;
    /// <summary>SoftwareClient or Macro_Deck_DIY_OLED_6_V1</summary>
    public string ProfileTarget { get; set; } = "SoftwareClient";
}

public class CreateFolderRequest
{
    [Required, MinLength(1)]
    public string DisplayName { get; set; } = string.Empty;
    /// <summary>Parent folder ID. Omit or null to add under root folder.</summary>
    public string? ParentFolderId { get; set; }
    public string ApplicationToTrigger { get; set; } = string.Empty;
}

public class CreateButtonRequest
{
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    /// <summary>Actions to execute on short press. Each item references PluginName + ActionClass + optional Configuration JSON.</summary>
    public List<ActionAssignmentRequest> Actions { get; set; } = new();
    public List<ActionAssignmentRequest> ActionsRelease { get; set; } = new();
    public List<ActionAssignmentRequest> ActionsLongPress { get; set; } = new();
    public List<ActionAssignmentRequest> ActionsLongPressRelease { get; set; } = new();
    public string? LabelOffText { get; set; }
    public string? LabelOnText { get; set; }
    public string? StateBindingVariable { get; set; }
}

public class ActionAssignmentRequest
{
    [Required]
    public string PluginName { get; set; } = string.Empty;
    [Required]
    public string ActionClass { get; set; } = string.Empty;
    /// <summary>Plugin-specific configuration as a JSON string.</summary>
    public string Configuration { get; set; } = string.Empty;
}

public class UpsertVariableRequest
{
    [Required, MinLength(1)]
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    /// <summary>Integer, Float, String, or Bool</summary>
    public string Type { get; set; } = "String";
    public string Creator { get; set; } = "User";
}

public class UpdateConfigRequest
{
    public bool? AutoStart { get; set; }
    public bool? AutoUpdates { get; set; }
    public bool? UpdateBetaVersions { get; set; }
    public bool? EnableAdbServer { get; set; }
    public bool? EnableAdbAutoStartApp { get; set; }
    public bool? AskOnNewConnections { get; set; }
    public bool? BlockNewConnections { get; set; }
    public string? Language { get; set; }
}

public class AssignProfileRequest
{
    [Required]
    public string ProfileId { get; set; } = string.Empty;
}

public class InstallExtensionRequest
{
    [Required]
    public string PackageId { get; set; } = string.Empty;
}
