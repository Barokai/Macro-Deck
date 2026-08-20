using MacroDeck.Server.Dto;
using MacroDeck.Server.Dto.Requests;
using MacroDeck.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace MacroDeck.Server.Controllers.Admin;

[ApiController]
[Route("api/profiles")]
public class ProfilesController : ControllerBase
{
    private readonly IProfileAdminService _profiles;

    public ProfilesController(IProfileAdminService profiles)
    {
        _profiles = profiles;
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<ProfileDto>> GetProfiles() =>
        Ok(_profiles.ListProfiles());

    [HttpGet("{profileId}")]
    public ActionResult<ProfileDto> GetProfile(string profileId)
    {
        var profile = _profiles.GetProfile(profileId);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPost]
    public ActionResult<ProfileDto> CreateProfile([FromBody] CreateProfileRequest request)
    {
        var created = _profiles.CreateProfile(request);
        return CreatedAtAction(nameof(GetProfile), new { profileId = created.ProfileId }, created);
    }

    [HttpDelete("{profileId}")]
    public IActionResult DeleteProfile(string profileId)
    {
        return _profiles.DeleteProfile(profileId) ? NoContent() : NotFound();
    }

    // --- Folders ---

    [HttpGet("{profileId}/folders")]
    public ActionResult<IReadOnlyList<FolderDto>> GetFolders(string profileId)
    {
        if (_profiles.GetProfile(profileId) is null) return NotFound();
        return Ok(_profiles.ListFolders(profileId));
    }

    [HttpGet("{profileId}/folders/{folderId}")]
    public ActionResult<FolderDto> GetFolder(string profileId, string folderId)
    {
        var folder = _profiles.GetFolder(profileId, folderId);
        return folder is null ? NotFound() : Ok(folder);
    }

    [HttpPost("{profileId}/folders")]
    public ActionResult<FolderDto> CreateFolder(string profileId, [FromBody] CreateFolderRequest request)
    {
        if (_profiles.GetProfile(profileId) is null) return NotFound("Profile not found.");
        var folder = _profiles.CreateFolder(profileId, request);
        if (folder is null) return Conflict("A folder with that name already exists or parent was not found.");
        return CreatedAtAction(nameof(GetFolder), new { profileId, folderId = folder.FolderId }, folder);
    }

    [HttpDelete("{profileId}/folders/{folderId}")]
    public IActionResult DeleteFolder(string profileId, string folderId)
    {
        return _profiles.DeleteFolder(profileId, folderId) ? NoContent() : NotFound();
    }

    // --- Buttons ---

    [HttpGet("{profileId}/folders/{folderId}/buttons")]
    public ActionResult<IReadOnlyList<ButtonDto>> GetButtons(string profileId, string folderId)
    {
        if (_profiles.GetFolder(profileId, folderId) is null) return NotFound();
        return Ok(_profiles.ListButtons(profileId, folderId));
    }

    [HttpGet("{profileId}/folders/{folderId}/buttons/{buttonGuid}")]
    public ActionResult<ButtonDto> GetButton(string profileId, string folderId, string buttonGuid)
    {
        var button = _profiles.GetButton(profileId, folderId, buttonGuid);
        return button is null ? NotFound() : Ok(button);
    }

    [HttpPost("{profileId}/folders/{folderId}/buttons")]
    public ActionResult<ButtonDto> CreateButton(string profileId, string folderId,
        [FromBody] CreateButtonRequest request)
    {
        if (_profiles.GetFolder(profileId, folderId) is null) return NotFound("Folder not found.");
        var button = _profiles.CreateButton(profileId, folderId, request);
        if (button is null) return Conflict("A button at that position already exists or action class was not found.");
        return CreatedAtAction(nameof(GetButton),
            new { profileId, folderId, buttonGuid = button.Guid }, button);
    }

    [HttpPut("{profileId}/folders/{folderId}/buttons/{buttonGuid}")]
    public ActionResult<ButtonDto> UpdateButton(string profileId, string folderId, string buttonGuid,
        [FromBody] CreateButtonRequest request)
    {
        var button = _profiles.UpdateButton(profileId, folderId, buttonGuid, request);
        return button is null ? NotFound() : Ok(button);
    }

    [HttpDelete("{profileId}/folders/{folderId}/buttons/{buttonGuid}")]
    public IActionResult DeleteButton(string profileId, string folderId, string buttonGuid)
    {
        return _profiles.DeleteButton(profileId, folderId, buttonGuid) ? NoContent() : NotFound();
    }
}
