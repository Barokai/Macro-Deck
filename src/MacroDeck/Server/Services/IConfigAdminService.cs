using MacroDeck.Server.Dto;
using MacroDeck.Server.Dto.Requests;

namespace MacroDeck.Server.Services;

public interface IConfigAdminService
{
    ConfigDto GetConfig();
    ConfigDto UpdateConfig(UpdateConfigRequest request);
    string GetAdminApiKey();
}
