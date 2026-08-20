using MacroDeck.Server.Dto;
using MacroDeck.Server.Dto.Requests;

namespace MacroDeck.Server.Services;

public interface IProfileAdminService
{
    IReadOnlyList<ProfileDto> ListProfiles();
    ProfileDto? GetProfile(string profileId);
    ProfileDto CreateProfile(CreateProfileRequest request);
    bool DeleteProfile(string profileId);

    IReadOnlyList<FolderDto> ListFolders(string profileId);
    FolderDto? GetFolder(string profileId, string folderId);
    FolderDto? CreateFolder(string profileId, CreateFolderRequest request);
    bool DeleteFolder(string profileId, string folderId);

    IReadOnlyList<ButtonDto> ListButtons(string profileId, string folderId);
    ButtonDto? GetButton(string profileId, string folderId, string buttonGuid);
    ButtonDto? CreateButton(string profileId, string folderId, CreateButtonRequest request);
    ButtonDto? UpdateButton(string profileId, string folderId, string buttonGuid, CreateButtonRequest request);
    bool DeleteButton(string profileId, string folderId, string buttonGuid);
}
