using System.CommandLine;
using System.CommandLine.Invocation;

namespace MacroDeck.Cli.Commands;

public static class ConfigCommands
{
    public static Command GetConfig(Func<InvocationContext, ApiClient> clientFactory)
    {
        var cmd = new Command("get", "Get current MacroDeck configuration.");
        cmd.SetHandler(async ctx =>
        {
            Console.WriteLine(await clientFactory(ctx).GetAsync("api/config"));
        });
        return cmd;
    }

    public static Command UpdateConfig(Func<InvocationContext, ApiClient> clientFactory)
    {
        var autoUpdatesOpt = new Option<bool?>("--auto-updates", "Enable/disable automatic update checks.");
        var updateBetaOpt = new Option<bool?>("--update-beta-versions", "Enable/disable beta updates.");
        var adbOpt = new Option<bool?>("--enable-adb-server", "Enable/disable ADB server support.");
        var adbAutoStartOpt = new Option<bool?>("--enable-adb-auto-start-app", "Enable/disable auto-start of Android app via ADB.");
        var askConnectionsOpt = new Option<bool?>("--ask-on-new-connections", "Prompt when new devices attempt to connect.");
        var blockConnectionsOpt = new Option<bool?>("--block-new-connections", "Block new device connections.");
        var languageOpt = new Option<string?>("--language", "Set UI language (e.g. English, German).");
        var autoStartOpt = new Option<bool?>("--auto-start", "Enable/disable autostart.");
        var cmd = new Command("update", "Patch MacroDeck configuration (only supplied fields are changed).");
        cmd.AddOption(autoUpdatesOpt);
        cmd.AddOption(updateBetaOpt);
        cmd.AddOption(adbOpt);
        cmd.AddOption(adbAutoStartOpt);
        cmd.AddOption(askConnectionsOpt);
        cmd.AddOption(blockConnectionsOpt);
        cmd.AddOption(languageOpt);
        cmd.AddOption(autoStartOpt);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var patch = new Dictionary<string, object?>();
            var autoUpdates = ctx.ParseResult.GetValueForOption(autoUpdatesOpt);
            if (autoUpdates is not null) patch["autoUpdates"] = autoUpdates;
            var updateBeta = ctx.ParseResult.GetValueForOption(updateBetaOpt);
            if (updateBeta is not null) patch["updateBetaVersions"] = updateBeta;
            var adb = ctx.ParseResult.GetValueForOption(adbOpt);
            if (adb is not null) patch["enableAdbServer"] = adb;
            var adbAutoStart = ctx.ParseResult.GetValueForOption(adbAutoStartOpt);
            if (adbAutoStart is not null) patch["enableAdbAutoStartApp"] = adbAutoStart;
            var askConnections = ctx.ParseResult.GetValueForOption(askConnectionsOpt);
            if (askConnections is not null) patch["askOnNewConnections"] = askConnections;
            var blockConnections = ctx.ParseResult.GetValueForOption(blockConnectionsOpt);
            if (blockConnections is not null) patch["blockNewConnections"] = blockConnections;
            var language = ctx.ParseResult.GetValueForOption(languageOpt);
            if (language is not null) patch["language"] = language;
            var autoStart = ctx.ParseResult.GetValueForOption(autoStartOpt);
            if (autoStart is not null) patch["autoStart"] = autoStart;

            if (patch.Count == 0)
            {
                Console.Error.WriteLine("No fields provided. Use one or more supported options like --auto-start, --auto-updates, --enable-adb-server, --language, etc.");
                ctx.ExitCode = 1;
                return;
            }

            Console.WriteLine(await clientFactory(ctx).PatchAsync("api/config", patch));
        });
        return cmd;
    }
}
