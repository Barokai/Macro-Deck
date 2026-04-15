using System.ComponentModel;
using System.Text.Json.Nodes;
using MacroDeck.Mcp;
using ModelContextProtocol.Server;

/// <summary>MCP tools for managing MacroDeck profiles, folders, and action buttons.</summary>
[McpServerToolType]
public class ProfileTools
{
    private readonly MacroDeckApiClient _api;
    public ProfileTools(MacroDeckApiClient api) => _api = api;

    [McpServerTool, Description(
        "List all MacroDeck profiles. Returns an array of profiles with their ID, display name, grid dimensions (rows × columns), and folder count.")]
    public async Task<string> ListProfiles() =>
        await _api.GetJsonAsync("api/profiles");

    [McpServerTool, Description(
        "Get the full structure (all folders and buttons) of a specific profile by its profileId.")]
    public async Task<string> GetProfile(
        [Description("The profileId returned by list_profiles.")] string profileId) =>
        await _api.GetJsonAsync($"api/profiles/{profileId}");

    [McpServerTool, Description(
        "Create a new MacroDeck profile with the specified display name and optional grid layout.")]
    public async Task<string> CreateProfile(
        [Description("Human-readable name for the new profile.")] string displayName,
        [Description("Number of button rows (default 3).")] int rows = 3,
        [Description("Number of button columns (default 5).")] int columns = 5,
        [Description("Spacing in pixels between buttons (default 10).")] int buttonSpacing = 10,
        [Description("Corner radius of buttons in pixels (default 40).")] int buttonRadius = 40)
    {
        return await _api.PostJsonAsync("api/profiles", new
        {
            displayName, rows, columns, buttonSpacing, buttonRadius, buttonBackground = true
        });
    }

    [McpServerTool, Description("Delete a profile by its profileId. At least two profiles must exist.")]
    public async Task<string> DeleteProfile(
        [Description("The profileId to delete.")] string profileId)
    {
        var ok = await _api.DeleteAsync($"api/profiles/{profileId}");
        return ok ? "Profile deleted." : "Profile not found or cannot be deleted (must keep ≥1 profile).";
    }

    [McpServerTool, Description(
        "List all folders in a profile. Each folder has a folderId, displayName, button count, and child folder IDs.")]
    public async Task<string> ListFolders(
        [Description("The profileId of the profile.")] string profileId) =>
        await _api.GetJsonAsync($"api/profiles/{profileId}/folders");

    [McpServerTool, Description(
        "Create a new folder inside a profile. Optionally specify a parent folder ID (otherwise adds under root) and a Windows process name to auto-switch to this folder when that app is focused.")]
    public async Task<string> CreateFolder(
        [Description("The profileId of the target profile.")] string profileId,
        [Description("Display name for the new folder.")] string displayName,
        [Description("Optional parent folder ID. Omit to add under root.")] string? parentFolderId = null,
        [Description("Optional Windows process name (e.g. 'chrome') to trigger auto-switch to this folder.")] string applicationToTrigger = "")
    {
        return await _api.PostJsonAsync($"api/profiles/{profileId}/folders", new
        {
            displayName, parentFolderId, applicationToTrigger
        });
    }

    [McpServerTool, Description("Delete a folder and all its child folders and buttons.")]
    public async Task<string> DeleteFolder(
        [Description("The profileId of the profile.")] string profileId,
        [Description("The folderId to delete.")] string folderId)
    {
        var ok = await _api.DeleteAsync($"api/profiles/{profileId}/folders/{folderId}");
        return ok ? "Folder deleted." : "Folder not found or cannot delete root folder.";
    }

    [McpServerTool, Description(
        "List all action buttons in a folder. Each button shows its grid position, state, label text, and the list of assigned plugin actions.")]
    public async Task<string> ListButtons(
        [Description("The profileId of the profile.")] string profileId,
        [Description("The folderId of the folder.")] string folderId) =>
        await _api.GetJsonAsync($"api/profiles/{profileId}/folders/{folderId}/buttons");

    [McpServerTool, Description(
        "Create an action button at a grid position in a folder. Assign one or more plugin actions that execute when the button is pressed. " +
        "Each action needs a pluginName and actionClass (from list_plugins). " +
        "Optionally provide label text (supports Cottle templates with {variable_name} syntax) and a variable name to bind the button state to.")]
    public async Task<string> CreateButton(
        [Description("The profileId of the profile.")] string profileId,
        [Description("The folderId of the folder.")] string folderId,
        [Description("Zero-based column (X) position of the button in the grid.")] int positionX,
        [Description("Zero-based row (Y) position of the button in the grid.")] int positionY,
        [Description("JSON array of action objects to run on short press. Each: {\"pluginName\":\"...\",\"actionClass\":\"...\",\"configuration\":\"...\"}. Use empty array [] for a display-only button.")] string actionsJson = "[]",
        [Description("Label displayed when button state is OFF. Supports Cottle templates, e.g. '{volume_level}%'.")] string? labelOffText = null,
        [Description("Label displayed when button state is ON.")] string? labelOnText = null,
        [Description("Variable name to auto-sync button state to (e.g. 'my_toggle'). Leave empty to disable.")] string? stateBindingVariable = null)
    {
        var actions = System.Text.Json.JsonSerializer.Deserialize<object[]>(actionsJson) ?? [];
        return await _api.PostJsonAsync($"api/profiles/{profileId}/folders/{folderId}/buttons", new
        {
            positionX, positionY,
            actions,
            actionsRelease = Array.Empty<object>(),
            actionsLongPress = Array.Empty<object>(),
            actionsLongPressRelease = Array.Empty<object>(),
            labelOffText, labelOnText, stateBindingVariable
        });
    }

    [McpServerTool, Description(
        "Update an existing button with partial fields. Provide only what you want to change in updateJson, e.g. {\"labelOffText\":\"⬇ Download\\n{speed}\"}. Existing values are preserved for omitted fields.")]
    public async Task<string> UpdateButton(
        [Description("The profileId of the profile.")] string profileId,
        [Description("The folderId of the folder.")] string folderId,
        [Description("The buttonGuid to update.")] string buttonGuid,
        [Description("JSON patch object with fields to update. Example: {\"labelOffText\":\"My label\"}")] string updateJson)
    {
        JsonObject patch;
        try
        {
            patch = JsonNode.Parse(updateJson)?.AsObject()
                    ?? throw new ArgumentException("updateJson must be a JSON object.");
        }
        catch (Exception ex)
        {
            throw new ArgumentException(
                "updateJson is not valid JSON. Pass an object like {\"labelOffText\":\"⬇ Download\\n{speed}\"}.", ex);
        }

        var existingRaw = await _api.GetJsonAsync($"api/profiles/{profileId}/folders/{folderId}/buttons/{buttonGuid}");
        var merged = JsonNode.Parse(existingRaw)?.AsObject()
                     ?? throw new InvalidOperationException("Unable to load existing button state.");

        foreach (var (key, value) in patch)
        {
            merged[key] = value?.DeepClone();
        }

        return await _api.PutJsonAsync(
            $"api/profiles/{profileId}/folders/{folderId}/buttons/{buttonGuid}", merged);
    }

    [McpServerTool, Description("Delete a button from a folder.")]
    public async Task<string> DeleteButton(
        [Description("The profileId of the profile.")] string profileId,
        [Description("The folderId of the folder.")] string folderId,
        [Description("The buttonGuid to delete.")] string buttonGuid)
    {
        var ok = await _api.DeleteAsync($"api/profiles/{profileId}/folders/{folderId}/buttons/{buttonGuid}");
        return ok ? "Button deleted." : "Button not found.";
    }
}
