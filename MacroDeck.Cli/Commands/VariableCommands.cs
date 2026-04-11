using System.CommandLine;
using System.CommandLine.Invocation;

namespace MacroDeck.Cli.Commands;

public static class VariableCommands
{
    public static Command ListVariables(Func<InvocationContext, ApiClient> clientFactory)
    {
        var cmd = new Command("list", "List all variables.");
        cmd.SetHandler(async ctx =>
        {
            Console.WriteLine(await clientFactory(ctx).GetAsync("api/variables"));
        });
        return cmd;
    }

    public static Command GetVariable(Func<InvocationContext, ApiClient> clientFactory)
    {
        var nameArg = new Argument<string>("name", "Variable name.");
        var cmd = new Command("get", "Get a single variable.");
        cmd.AddArgument(nameArg);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var name = ctx.ParseResult.GetValueForArgument(nameArg);
            Console.WriteLine(await clientFactory(ctx).GetAsync($"api/variables/{Uri.EscapeDataString(name)}"));
        });
        return cmd;
    }

    public static Command SetVariable(Func<InvocationContext, ApiClient> clientFactory)
    {
        var nameArg = new Argument<string>("name", "Variable name.");
        var valueOpt = new Option<string>("--value", "Variable value.") { IsRequired = true };
        var typeOpt = new Option<string>("--type", () => "String", "Variable type: String, Integer, Float, Bool.");
        var creatorOpt = new Option<string>("--creator", () => "CLI", "Creator label.");
        var cmd = new Command("set", "Create or update a variable.");
        cmd.AddArgument(nameArg);
        cmd.AddOption(valueOpt);
        cmd.AddOption(typeOpt);
        cmd.AddOption(creatorOpt);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var name = ctx.ParseResult.GetValueForArgument(nameArg);
            var result = await clientFactory(ctx).PutAsync($"api/variables/{Uri.EscapeDataString(name)}", new
            {
                value = ctx.ParseResult.GetValueForOption(valueOpt),
                type = ctx.ParseResult.GetValueForOption(typeOpt),
                creator = ctx.ParseResult.GetValueForOption(creatorOpt),
            });
            Console.WriteLine(result);
        });
        return cmd;
    }

    public static Command DeleteVariable(Func<InvocationContext, ApiClient> clientFactory)
    {
        var nameArg = new Argument<string>("name", "Variable name.");
        var cmd = new Command("delete", "Delete a variable.");
        cmd.AddArgument(nameArg);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var name = ctx.ParseResult.GetValueForArgument(nameArg);
            var ok = await clientFactory(ctx).DeleteAsync($"api/variables/{Uri.EscapeDataString(name)}");
            Console.WriteLine(ok ? "Variable deleted." : "Not found.");
        });
        return cmd;
    }
}
