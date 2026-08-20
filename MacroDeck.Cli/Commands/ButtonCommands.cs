using System.CommandLine;
using System.CommandLine.Invocation;

namespace MacroDeck.Cli.Commands;

public static class ButtonCommands
{
    public static Command ListButtons(Func<InvocationContext, ApiClient> clientFactory)
    {
        var profileIdArg = new Argument<string>("profileId", "Profile ID.");
        var folderIdArg = new Argument<string>("folderId", "Folder ID.");
        var cmd = new Command("list", "List all buttons in a folder.");
        cmd.AddArgument(profileIdArg);
        cmd.AddArgument(folderIdArg);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var pid = ctx.ParseResult.GetValueForArgument(profileIdArg);
            var fid = ctx.ParseResult.GetValueForArgument(folderIdArg);
            Console.WriteLine(await clientFactory(ctx).GetAsync($"api/profiles/{pid}/folders/{fid}/buttons"));
        });
        return cmd;
    }

    public static Command CreateButton(Func<InvocationContext, ApiClient> clientFactory)
    {
        var profileIdArg = new Argument<string>("profileId", "Profile ID.");
        var folderIdArg = new Argument<string>("folderId", "Folder ID.");
        var xOpt = new Option<int>("--x", "Column (0-based).") { IsRequired = true };
        var yOpt = new Option<int>("--y", "Row (0-based).") { IsRequired = true };
        var labelOpt = new Option<string>("--label", () => "", "Off-state label text.");
        var labelOnOpt = new Option<string>("--label-on", () => "", "On-state label text.");
        var actionsOpt = new Option<string>("--actions-json", () => "[]",
            "JSON array of action assignments: [{\"pluginName\":\"...\",\"actionClass\":\"...\",\"configuration\":{...}}]");
        var cmd = new Command("create", "Create a button at a grid position.");
        cmd.AddArgument(profileIdArg);
        cmd.AddArgument(folderIdArg);
        cmd.AddOption(xOpt);
        cmd.AddOption(yOpt);
        cmd.AddOption(labelOpt);
        cmd.AddOption(labelOnOpt);
        cmd.AddOption(actionsOpt);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var pid = ctx.ParseResult.GetValueForArgument(profileIdArg);
            var fid = ctx.ParseResult.GetValueForArgument(folderIdArg);
            var actionsJson = ctx.ParseResult.GetValueForOption(actionsOpt) ?? "[]";
            object actions;
            try { actions = System.Text.Json.JsonSerializer.Deserialize<object>(actionsJson)!; }
            catch { Console.Error.WriteLine("--actions-json is not valid JSON."); ctx.ExitCode = 1; return; }

            var result = await clientFactory(ctx).PostAsync(
                $"api/profiles/{pid}/folders/{fid}/buttons",
                new
                {
                    positionX = ctx.ParseResult.GetValueForOption(xOpt),
                    positionY = ctx.ParseResult.GetValueForOption(yOpt),
                    labelOffText = ctx.ParseResult.GetValueForOption(labelOpt),
                    labelOnText = ctx.ParseResult.GetValueForOption(labelOnOpt),
                    actions = actions,
                });
            Console.WriteLine(result);
        });
        return cmd;
    }

    public static Command DeleteButton(Func<InvocationContext, ApiClient> clientFactory)
    {
        var profileIdArg = new Argument<string>("profileId", "Profile ID.");
        var folderIdArg = new Argument<string>("folderId", "Folder ID.");
        var guidArg = new Argument<string>("buttonGuid", "Button GUID.");
        var cmd = new Command("delete", "Delete a button.");
        cmd.AddArgument(profileIdArg);
        cmd.AddArgument(folderIdArg);
        cmd.AddArgument(guidArg);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var pid = ctx.ParseResult.GetValueForArgument(profileIdArg);
            var fid = ctx.ParseResult.GetValueForArgument(folderIdArg);
            var guid = ctx.ParseResult.GetValueForArgument(guidArg);
            var ok = await clientFactory(ctx).DeleteAsync($"api/profiles/{pid}/folders/{fid}/buttons/{guid}");
            Console.WriteLine(ok ? "Button deleted." : "Not found.");
        });
        return cmd;
    }
}
