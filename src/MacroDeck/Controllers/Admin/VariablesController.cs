using MacroDeck.Server.Dto;
using MacroDeck.Server.Dto.Requests;
using MacroDeck.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace MacroDeck.Server.Controllers.Admin;

[ApiController]
[Route("api/variables")]
public class VariablesController : ControllerBase
{
    private readonly IVariableAdminService _variables;

    public VariablesController(IVariableAdminService variables)
    {
        _variables = variables;
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<VariableDto>> GetVariables() =>
        Ok(_variables.ListVariables());

    [HttpGet("{name}")]
    public ActionResult<VariableDto> GetVariable(string name)
    {
        var variable = _variables.GetVariable(name);
        return variable is null ? NotFound() : Ok(variable);
    }

    [HttpPut]
    public ActionResult<VariableDto> UpsertVariable([FromBody] UpsertVariableRequest request) =>
        Ok(_variables.UpsertVariable(request));

    [HttpDelete("{name}")]
    public IActionResult DeleteVariable(string name)
    {
        return _variables.DeleteVariable(name) ? NoContent() : NotFound();
    }
}
