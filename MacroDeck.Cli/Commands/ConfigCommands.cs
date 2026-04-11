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
        var hostOpt = new Option<string?>("--host", "Host / bind address.");
        var portOpt = new Option<int?>("--port", "Port number.");
        var adbOpt = new Option<bool?>("--adb", "Enable/disable ADB support.");
        var sslOpt = new Option<bool?>("--ssl", "Enable/disable SSL.");
        var autoStartOpt = new Option<bool?>("--auto-start", "Enable/disable autostart.");
        var cmd = new Command("update", "Patch MacroDeck configuration (only supplied fields are changed).");
        cmd.AddOption(hostOpt);
        cmd.AddOption(portOpt);
        cmd.AddOption(adbOpt);
        cmd.AddOption(sslOpt);
        cmd.AddOption(autoStartOpt);
        cmd.SetHandler(async (InvocationContext ctx) =>
        {
            var patch = new Dictionary<string, object?>();
            var host = ctx.ParseResult.GetValueForOption(hostOpt);
            if (host is not null) patch["host"] = host;
            var port = ctx.ParseResult.GetValueForOption(portOpt);
            if (port is not null) patch["port"] = port;
            var adb = ctx.ParseResult.GetValueForOption(adbOpt);
            if (adb is not null) patch["adbSupport"] = adb;
            var ssl = ctx.ParseResult.GetValueForOption(sslOpt);
            if (ssl is not null) patch["ssl"] = ssl;
            var autoStart = ctx.ParseResult.GetValueForOption(autoStartOpt);
            if (autoStart is not null) patch["autoStart"] = autoStart;

            if (patch.Count == 0)
            {
                Console.Error.WriteLine("No fields provided. Use --host, --port, --adb, --ssl, or --auto-start.");
                ctx.ExitCode = 1;
                return;
            }

            Console.WriteLine(await clientFactory(ctx).PatchAsync("api/config", patch));
        });
        return cmd;
    }
}
