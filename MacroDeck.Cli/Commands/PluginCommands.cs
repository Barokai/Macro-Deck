using System.CommandLine;
using System.CommandLine.Invocation;

namespace MacroDeck.Cli.Commands;

public static class PluginCommands
{
    public static Command ListPlugins(Func<InvocationContext, ApiClient> clientFactory)
    {
        var cmd = new Command("list", "List all installed plugins.");
        cmd.SetHandler(async ctx =>
        {
            Console.WriteLine(await clientFactory(ctx).GetAsync("api/plugins"));
        });
        return cmd;
    }

    public static Command GetPlugin(Func<InvocationContext, ApiClient> clientFactory)
    {
        var nameArg = new Argument<string>("name", "Plugin name.");
        var cmd = new Command("get", "Get details and available actions for a plugin.");
        cmd.AddArgument(nameArg);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var name = ctx.ParseResult.GetValueForArgument(nameArg);
            Console.WriteLine(await clientFactory(ctx).GetAsync($"api/plugins/{Uri.EscapeDataString(name)}"));
        });
        return cmd;
    }

    public static Command SearchStore(Func<InvocationContext, ApiClient> clientFactory)
    {
        var queryArg = new Argument<string>("query", "Search term.");
        var cmd = new Command("search", "Search the MacroDeck extension store.");
        cmd.AddArgument(queryArg);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var q = Uri.EscapeDataString(ctx.ParseResult.GetValueForArgument(queryArg));
            Console.WriteLine(await clientFactory(ctx).GetAsync($"api/plugins/store/search?q={q}"));
        });
        return cmd;
    }

    public static Command InstallPlugin(Func<InvocationContext, ApiClient> clientFactory)
    {
        var idArg = new Argument<string>("extensionId", "Extension store ID to install.");
        var cmd = new Command("install", "Install a plugin from the extension store.");
        cmd.AddArgument(idArg);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var id = ctx.ParseResult.GetValueForArgument(idArg);
            Console.WriteLine(await clientFactory(ctx).PostAsync("api/plugins/store/install", new { extensionId = id }));
        });
        return cmd;
    }
}
