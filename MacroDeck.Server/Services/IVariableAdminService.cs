using MacroDeck.Server.Dto;
using MacroDeck.Server.Dto.Requests;

namespace MacroDeck.Server.Services;

public interface IVariableAdminService
{
    IReadOnlyList<VariableDto> ListVariables();
    VariableDto? GetVariable(string name);
    VariableDto UpsertVariable(UpsertVariableRequest request);
    bool DeleteVariable(string name);
}
