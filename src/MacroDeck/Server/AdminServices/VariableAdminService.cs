using MacroDeck.Server.Dto;
using MacroDeck.Server.Dto.Requests;
using MacroDeck.Server.Services;
using SuchByte.MacroDeck.Variables;

namespace SuchByte.MacroDeck.Server.AdminServices;

public class VariableAdminService : IVariableAdminService
{
    public IReadOnlyList<VariableDto> ListVariables() =>
        VariableManager.Variables.Select(ToDto).ToList();

    public VariableDto? GetVariable(string name)
    {
        var v = VariableManager.Variables
            .FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        return v is null ? null : ToDto(v);
    }

    public VariableDto UpsertVariable(UpsertVariableRequest request)
    {
        if (!Enum.TryParse<VariableType>(request.Type, ignoreCase: true, out var type))
            type = VariableType.String;

        VariableManager.SetValue(request.Name, request.Value, type, request.Creator);
        return new VariableDto
        {
            Name = VariableManager.ConvertNameString(request.Name),
            Value = request.Value,
            Type = type.ToString(),
            Creator = request.Creator,
        };
    }

    public bool DeleteVariable(string name)
    {
        var exists = VariableManager.Variables
            .Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        if (!exists) return false;
        VariableManager.DeleteVariable(name);
        return true;
    }

    private static VariableDto ToDto(Variable v) => new()
    {
        Name = v.Name,
        Value = v.Value,
        Type = v.Type,
        Creator = v.Creator,
    };
}
