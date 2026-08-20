using System.CommandLine;
using System.CommandLine.Invocation;

namespace MacroDeck.Cli.Commands;

public static class DeviceCommands
{
    public static Command ListDevices(Func<InvocationContext, ApiClient> clientFactory)
    {
        var cmd = new Command("list", "List known devices.");
        cmd.SetHandler(async ctx =>
        {
            Console.WriteLine(await clientFactory(ctx).GetAsync("api/devices"));
        });
        return cmd;
    }

    public static Command AssignProfile(Func<InvocationContext, ApiClient> clientFactory)
    {
        var clientIdArg = new Argument<string>("clientId", "Device client ID.");
        var profileIdOpt = new Option<string>("--profile", "Profile ID to assign.") { IsRequired = true };
        var cmd = new Command("assign-profile", "Assign a profile to a device.");
        cmd.AddArgument(clientIdArg);
        cmd.AddOption(profileIdOpt);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var cid = ctx.ParseResult.GetValueForArgument(clientIdArg);
            var result = await clientFactory(ctx).PutAsync($"api/devices/{Uri.EscapeDataString(cid)}/profile", new
            {
                profileId = ctx.ParseResult.GetValueForOption(profileIdOpt),
            });
            Console.WriteLine(result);
        });
        return cmd;
    }

    public static Command SetBlocked(Func<InvocationContext, ApiClient> clientFactory)
    {
        var clientIdArg = new Argument<string>("clientId", "Device client ID.");
        var blockedOpt = new Option<bool>("--blocked", "true to block, false to unblock.") { IsRequired = true };
        var cmd = new Command("set-blocked", "Block or unblock a device.");
        cmd.AddArgument(clientIdArg);
        cmd.AddOption(blockedOpt);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var cid = ctx.ParseResult.GetValueForArgument(clientIdArg);
            var blocked = ctx.ParseResult.GetValueForOption(blockedOpt);
            var result = await clientFactory(ctx)
                .PutAsync($"api/devices/{Uri.EscapeDataString(cid)}/blocked?blocked={blocked.ToString().ToLowerInvariant()}", new { });
            Console.WriteLine(result);
        });
        return cmd;
    }
}
