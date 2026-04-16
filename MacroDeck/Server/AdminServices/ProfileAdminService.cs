using MacroDeck.Server.Dto;
using MacroDeck.Server.Dto.Requests;
using MacroDeck.Server.Services;
using System.Drawing;
using SuchByte.MacroDeck.Device;
using SuchByte.MacroDeck.Folders;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Profiles;

namespace SuchByte.MacroDeck.Server.AdminServices;

public class ProfileAdminService : IProfileAdminService
{
    // ---------- Profiles ----------

    public IReadOnlyList<ProfileDto> ListProfiles() =>
        ProfileManager.Profiles.Select(ToDto).ToList();

    public ProfileDto? GetProfile(string profileId) =>
        ProfileManager.Profiles.Select(ToDto).FirstOrDefault(p => p.ProfileId == profileId);

    public ProfileDto CreateProfile(CreateProfileRequest request)
    {
        var profile = ProfileManager.CreateProfile(request.DisplayName);
        profile.Rows = request.Rows;
        profile.Columns = request.Columns;
        profile.ButtonSpacing = request.ButtonSpacing;
        profile.ButtonRadius = request.ButtonRadius;
        profile.ButtonBackground = request.ButtonBackground;
        if (Enum.TryParse<DeviceClass>(request.ProfileTarget, out var deviceClass))
            profile.ProfileTarget = deviceClass;
        ProfileManager.Save();
        return ToDto(profile);
    }

    public bool DeleteProfile(string profileId)
    {
        var profile = ProfileManager.FindProfileById(profileId);
        if (profile is null) return false;
        ProfileManager.DeleteProfile(profile);
        return true;
    }

    // ---------- Folders ----------

    public IReadOnlyList<FolderDto> ListFolders(string profileId)
    {
        var profile = ProfileManager.FindProfileById(profileId);
        return profile is null ? [] : profile.Folders.Select(ToDto).ToList();
    }

    public FolderDto? GetFolder(string profileId, string folderId)
    {
        var profile = ProfileManager.FindProfileById(profileId);
        if (profile is null) return null;
        var folder = ProfileManager.FindFolderById(folderId, profile);
        return folder is null ? null : ToDto(folder);
    }

    public FolderDto? CreateFolder(string profileId, CreateFolderRequest request)
    {
        var profile = ProfileManager.FindProfileById(profileId);
        if (profile is null) return null;

        var parent = string.IsNullOrWhiteSpace(request.ParentFolderId)
            ? profile.Folders.FirstOrDefault(f => f.IsRootFolder)
            : ProfileManager.FindFolderById(request.ParentFolderId, profile);
        if (parent is null) return null;

        var folder = ProfileManager.CreateFolder(request.DisplayName, parent, profile);
        if (folder is null) return null;

        if (!string.IsNullOrWhiteSpace(request.ApplicationToTrigger))
        {
            folder.ApplicationToTrigger = request.ApplicationToTrigger;
            ProfileManager.Save();
        }
        return ToDto(folder);
    }

    public bool DeleteFolder(string profileId, string folderId)
    {
        var profile = ProfileManager.FindProfileById(profileId);
        if (profile is null) return false;
        var folder = ProfileManager.FindFolderById(folderId, profile);
        if (folder is null) return false;
        ProfileManager.DeleteFolder(folder, profile);
        return true;
    }

    // ---------- Buttons ----------

    public IReadOnlyList<ButtonDto> ListButtons(string profileId, string folderId)
    {
        var folder = ResolveFolder(profileId, folderId);
        return folder is null ? [] : folder.ActionButtons.Select(ToDto).ToList();
    }

    public ButtonDto? GetButton(string profileId, string folderId, string buttonGuid)
    {
        var folder = ResolveFolder(profileId, folderId);
        var btn = folder?.ActionButtons.FirstOrDefault(b => b.Guid == buttonGuid);
        return btn is null ? null : ToDto(btn);
    }

    public ButtonDto? CreateButton(string profileId, string folderId, CreateButtonRequest request)
    {
        var folder = ResolveFolder(profileId, folderId);
        if (folder is null) return null;
        if (folder.ActionButtons.Any(b => b.Position_X == request.PositionX && b.Position_Y == request.PositionY))
            return null;

        var button = new ActionButton.ActionButton
        {
            Guid = System.Guid.NewGuid().ToString(),
            Position_X = request.PositionX,
            Position_Y = request.PositionY,
            LabelOff = new ActionButton.ButtonLabel { LabelText = request.LabelOffText ?? string.Empty },
            LabelOn = new ActionButton.ButtonLabel { LabelText = request.LabelOnText ?? string.Empty },
            StateBindingVariable = request.StateBindingVariable ?? string.Empty,
            Actions = ResolveActions(request.Actions),
            ActionsRelease = ResolveActions(request.ActionsRelease),
            ActionsLongPress = ResolveActions(request.ActionsLongPress),
            ActionsLongPressRelease = ResolveActions(request.ActionsLongPressRelease),
        };

        ApplyButtonStyle(button, request);

        folder.ActionButtons.Add(button);
        ProfileManager.Save();
        return ToDto(button);
    }

    public ButtonDto? UpdateButton(string profileId, string folderId, string buttonGuid, CreateButtonRequest request)
    {
        var folder = ResolveFolder(profileId, folderId);
        var button = folder?.ActionButtons.FirstOrDefault(b => b.Guid == buttonGuid);
        if (button is null) return null;

        button.Position_X = request.PositionX;
        button.Position_Y = request.PositionY;
        if (button.LabelOff != null) button.LabelOff.LabelText = request.LabelOffText ?? string.Empty;
        if (button.LabelOn != null) button.LabelOn.LabelText = request.LabelOnText ?? string.Empty;
        button.StateBindingVariable = request.StateBindingVariable ?? string.Empty;
        button.Actions = ResolveActions(request.Actions);
        button.ActionsRelease = ResolveActions(request.ActionsRelease);
        button.ActionsLongPress = ResolveActions(request.ActionsLongPress);
        button.ActionsLongPressRelease = ResolveActions(request.ActionsLongPressRelease);

        ApplyButtonStyle(button, request);

        ProfileManager.Save();
        return ToDto(button);
    }

    public bool DeleteButton(string profileId, string folderId, string buttonGuid)
    {
        var folder = ResolveFolder(profileId, folderId);
        var button = folder?.ActionButtons.FirstOrDefault(b => b.Guid == buttonGuid);
        if (button is null) return false;
        button.Dispose();
        folder!.ActionButtons.Remove(button);
        ProfileManager.Save();
        return true;
    }

    // ---------- Helpers ----------

    private static MacroDeckFolder? ResolveFolder(string profileId, string folderId)
    {
        var profile = ProfileManager.FindProfileById(profileId);
        return profile is null ? null : ProfileManager.FindFolderById(folderId, profile);
    }

    private static List<PluginAction> ResolveActions(List<ActionAssignmentRequest> requests) =>
        requests
            .Select(r =>
            {
                // Prefer explicit plugin name, but support legacy payloads where pluginName is empty.
                var pluginFound = !string.IsNullOrWhiteSpace(r.PluginName)
                    && PluginManager.Plugins.TryGetValue(r.PluginName, out var namedPlugin);

                var plugin = pluginFound
                    ? namedPlugin
                    : PluginManager.Plugins.Values.FirstOrDefault(p =>
                        p.Actions.Any(a => a.GetType().Name == r.ActionClass));

                if (plugin is null) return null;

                var action = plugin.Actions.FirstOrDefault(a => a.GetType().Name == r.ActionClass);
                if (action is null) return null;

                var instance = (PluginAction)Activator.CreateInstance(action.GetType())!;
                instance.Configuration = r.Configuration;
                return instance;
            })
            .Where(a => a is not null)
            .ToList()!;

    private static void ApplyButtonStyle(ActionButton.ActionButton button, CreateButtonRequest request)
    {
        if (button.LabelOff == null) button.LabelOff = new ActionButton.ButtonLabel();
        if (button.LabelOn == null) button.LabelOn = new ActionButton.ButtonLabel();

        var iconOff = BuildIconString(request.IconPack, request.IconName) ?? request.IconOff;
        var iconOn = BuildIconString(request.IconPack, request.IconNameOn) ?? request.IconOn;

        if (!string.IsNullOrWhiteSpace(iconOff)) button.IconOff = iconOff;
        if (!string.IsNullOrWhiteSpace(iconOn)) button.IconOn = iconOn;

        if (TryParseColor(request.BackgroundColorOff, out var bgOff)) button.BackColorOff = bgOff;
        if (TryParseColor(request.BackgroundColorOn, out var bgOn)) button.BackColorOn = bgOn;
        if (TryParseColor(request.LabelColorOff, out var labelOff)) button.LabelOff.LabelColor = labelOff;
        if (TryParseColor(request.LabelColorOn, out var labelOn)) button.LabelOn.LabelColor = labelOn;
    }

    private static bool TryParseColor(string? value, out Color color)
    {
        color = Color.Empty;
        if (string.IsNullOrWhiteSpace(value)) return false;
        try
        {
            color = ColorTranslator.FromHtml(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string? BuildIconString(string? iconPack, string? iconName)
    {
        if (string.IsNullOrWhiteSpace(iconPack) || string.IsNullOrWhiteSpace(iconName)) return null;
        return $"{iconPack}.{iconName}";
    }

    private static string ToHex(Color color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

    private static (string? pack, string? name) SplitIconString(string? icon)
    {
        if (string.IsNullOrWhiteSpace(icon)) return (null, null);
        var idx = icon.IndexOf('.');
        if (idx <= 0 || idx == icon.Length - 1) return (null, null);
        return (icon[..idx], icon[(idx + 1)..]);
    }

    private static ProfileDto ToDto(MacroDeckProfile p) => new()
    {
        ProfileId = p.ProfileId,
        DisplayName = p.DisplayName,
        Rows = p.Rows,
        Columns = p.Columns,
        ButtonSpacing = p.ButtonSpacing,
        ButtonRadius = p.ButtonRadius,
        ButtonBackground = p.ButtonBackground,
        ProfileTarget = p.ProfileTarget.ToString(),
        FolderCount = p.Folders.Count,
    };

    private static FolderDto ToDto(MacroDeckFolder f) => new()
    {
        FolderId = f.FolderId,
        DisplayName = f.DisplayName,
        IsRootFolder = f.IsRootFolder,
        ChildFolderIds = f.Childs,
        ButtonCount = f.ActionButtons.Count,
        ApplicationToTrigger = f.ApplicationToTrigger ?? string.Empty,
    };

    private static ButtonDto ToDto(ActionButton.ActionButton b)
    {
        var (iconPack, iconName) = SplitIconString(b.IconOff);
        var (_, iconNameOn) = SplitIconString(b.IconOn);

        return new ButtonDto
        {
            Guid = b.Guid,
            PositionX = b.Position_X,
            PositionY = b.Position_Y,
            State = b.State,
            LabelOffText = b.LabelOff?.LabelText,
            LabelOnText = b.LabelOn?.LabelText,
            StateBindingVariable = b.StateBindingVariable,
            IconPack = iconPack,
            IconName = iconName,
            IconNameOn = iconNameOn,
            IconOff = b.IconOff,
            IconOn = b.IconOn,
            BackgroundColorOff = ToHex(b.BackColorOff),
            BackgroundColorOn = ToHex(b.BackColorOn),
            LabelColorOff = b.LabelOff is null ? null : ToHex(b.LabelOff.LabelColor),
            LabelColorOn = b.LabelOn is null ? null : ToHex(b.LabelOn.LabelColor),
            Actions = b.Actions?.Select(ActionToDto).ToList() ?? [],
            ActionsRelease = b.ActionsRelease?.Select(ActionToDto).ToList() ?? [],
            ActionsLongPress = b.ActionsLongPress?.Select(ActionToDto).ToList() ?? [],
            ActionsLongPressRelease = b.ActionsLongPressRelease?.Select(ActionToDto).ToList() ?? [],
        };
    }

    private static ActionDto ActionToDto(PluginAction a) => new()
    {
        PluginName = PluginManager.Plugins
            .FirstOrDefault(kv => kv.Value.Actions.Any(pa => pa.GetType().Name == a.GetType().Name)).Key
            ?? string.Empty,
        ActionClass = a.GetType().Name,
        Configuration = a.Configuration ?? string.Empty,
        ConfigurationSummary = a.ConfigurationSummary ?? string.Empty,
    };
}
