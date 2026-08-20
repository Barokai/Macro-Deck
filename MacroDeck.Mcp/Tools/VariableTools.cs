using System.ComponentModel;
using MacroDeck.Mcp;
using ModelContextProtocol.Server;

/// <summary>MCP tools for managing MacroDeck variables.</summary>
[McpServerToolType]
public class VariableTools
{
    private readonly MacroDeckApiClient _api;
    public VariableTools(MacroDeckApiClient api) => _api = api;

    [McpServerTool, Description(
        "List all MacroDeck variables. Variables can be referenced in button label templates using {variable_name} syntax. " +
        "Returns name, current value, type (Integer/Float/String/Bool), and the creator plugin.")]
    public async Task<string> ListVariables() =>
        await _api.GetJsonAsync("api/variables");

    [McpServerTool, Description("Get the current value and metadata of a single variable by name.")]
    public async Task<string> GetVariable(
        [Description("The exact variable name (case-insensitive).")] string name) =>
        await _api.GetJsonAsync($"api/variables/{Uri.EscapeDataString(name)}");

    [McpServerTool, Description(
        "Create or update a MacroDeck variable. " +
        "Variables can be used to drive button label templates and state bindings. " +
        "Type must be one of: Integer, Float, String, Bool.")]
    public async Task<string> SetVariable(
        [Description("Variable name (letters, numbers, underscores).")] string name,
        [Description("New value to set.")] string value,
        [Description("Data type: Integer, Float, String, or Bool. Default is String.")] string type = "String",
        [Description("Creator label (default 'User').")] string creator = "User") =>
        await _api.PutJsonAsync("api/variables", new { name, value, type, creator });

    [McpServerTool, Description("Delete a variable by name.")]
    public async Task<string> DeleteVariable(
        [Description("The variable name to delete.")] string name)
    {
        var ok = await _api.DeleteAsync($"api/variables/{Uri.EscapeDataString(name)}");
        return ok ? "Variable deleted." : "Variable not found.";
    }
}
