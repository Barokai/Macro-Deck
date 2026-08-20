using System.CommandLine;
using System.CommandLine.Invocation;

namespace MacroDeck.Cli.Commands;

public static class ProfileCommands
{
    public static Command ListProfiles(Func<InvocationContext, ApiClient> clientFactory)
    {
        var cmd = new Command("list", "List all profiles.");
        cmd.SetHandler(async ctx =>
        {
            var result = await clientFactory(ctx).GetAsync("api/profiles");
            Console.WriteLine(result);
        });
        return cmd;
    }

    public static Command GetProfile(Func<InvocationContext, ApiClient> clientFactory)
    {
        var profileIdArg = new Argument<string>("profileId", "Profile ID to inspect.");
        var cmd = new Command("get", "Get a profile with its folders.");
        cmd.AddArgument(profileIdArg);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var id = ctx.ParseResult.GetValueForArgument(profileIdArg);
            Console.WriteLine(await clientFactory(ctx).GetAsync($"api/profiles/{id}"));
        });
        return cmd;
    }

    public static Command CreateProfile(Func<InvocationContext, ApiClient> clientFactory)
    {
        var nameOpt = new Option<string>("--name", "Profile display name.") { IsRequired = true };
        var rowsOpt = new Option<int>("--rows", () => 3, "Number of button rows.");
        var colsOpt = new Option<int>("--columns", () => 5, "Number of button columns.");
        var cmd = new Command("create", "Create a new profile.");
        cmd.AddOption(nameOpt);
        cmd.AddOption(rowsOpt);
        cmd.AddOption(colsOpt);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var result = await clientFactory(ctx).PostAsync("api/profiles", new
            {
                displayName = ctx.ParseResult.GetValueForOption(nameOpt),
                rows = ctx.ParseResult.GetValueForOption(rowsOpt),
                columns = ctx.ParseResult.GetValueForOption(colsOpt),
                buttonSpacing = 10,
                buttonRadius = 40,
                buttonBackground = true,
            });
            Console.WriteLine(result);
        });
        return cmd;
    }

    public static Command DeleteProfile(Func<InvocationContext, ApiClient> clientFactory)
    {
        var profileIdArg = new Argument<string>("profileId", "Profile ID to delete.");
        var cmd = new Command("delete", "Delete a profile.");
        cmd.AddArgument(profileIdArg);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var id = ctx.ParseResult.GetValueForArgument(profileIdArg);
            var ok = await clientFactory(ctx).DeleteAsync($"api/profiles/{id}");
            Console.WriteLine(ok ? "Deleted." : "Not found or cannot delete.");
        });
        return cmd;
    }

    public static Command ListFolders(Func<InvocationContext, ApiClient> clientFactory)
    {
        var profileIdArg = new Argument<string>("profileId", "Profile ID.");
        var cmd = new Command("list-folders", "List folders in a profile.");
        cmd.AddArgument(profileIdArg);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var id = ctx.ParseResult.GetValueForArgument(profileIdArg);
            Console.WriteLine(await clientFactory(ctx).GetAsync($"api/profiles/{id}/folders"));
        });
        return cmd;
    }

    public static Command CreateFolder(Func<InvocationContext, ApiClient> clientFactory)
    {
        var profileIdArg = new Argument<string>("profileId", "Profile ID.");
        var nameOpt = new Option<string>("--name", "Folder display name.") { IsRequired = true };
        var parentOpt = new Option<string?>("--parent", "Parent folder ID (omit for root).");
        var appOpt = new Option<string>("--app", () => "", "Windows process name to auto-switch to this folder.");
        var cmd = new Command("create-folder", "Create a folder inside a profile.");
        cmd.AddArgument(profileIdArg);
        cmd.AddOption(nameOpt);
        cmd.AddOption(parentOpt);
        cmd.AddOption(appOpt);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var id = ctx.ParseResult.GetValueForArgument(profileIdArg);
            var result = await clientFactory(ctx).PostAsync($"api/profiles/{id}/folders", new
            {
                displayName = ctx.ParseResult.GetValueForOption(nameOpt),
                parentFolderId = ctx.ParseResult.GetValueForOption(parentOpt),
                applicationToTrigger = ctx.ParseResult.GetValueForOption(appOpt),
            });
            Console.WriteLine(result);
        });
        return cmd;
    }

    public static Command DeleteFolder(Func<InvocationContext, ApiClient> clientFactory)
    {
        var profileIdArg = new Argument<string>("profileId", "Profile ID.");
        var folderIdArg = new Argument<string>("folderId", "Folder ID to delete.");
        var cmd = new Command("delete-folder", "Delete a folder and all its children.");
        cmd.AddArgument(profileIdArg);
        cmd.AddArgument(folderIdArg);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var pid = ctx.ParseResult.GetValueForArgument(profileIdArg);
            var fid = ctx.ParseResult.GetValueForArgument(folderIdArg);
            var ok = await clientFactory(ctx).DeleteAsync($"api/profiles/{pid}/folders/{fid}");
            Console.WriteLine(ok ? "Folder deleted." : "Not found.");
        });
        return cmd;
    }
}
