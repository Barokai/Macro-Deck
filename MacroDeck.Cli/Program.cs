using System.CommandLine;
using System.CommandLine.Invocation;
using MacroDeck.Cli;
using MacroDeck.Cli.Commands;

// Global options shared by all commands
var urlOption = new Option<string?>("--url", "MacroDeck base URL (default: http://localhost:8191 or MACRODECK_URL env var)");
var keyOption = new Option<string?>("--key", "Admin API key (default: MACRODECK_API_KEY env var)");

var root = new RootCommand("MacroDeck CLI – manage profiles, plugins, variables, and devices.")
{
    urlOption,
    keyOption,
};

// Helper to build an ApiClient from the parsed global options
ApiClient BuildClient(InvocationContext ctx) =>
    ApiClient.FromEnvironment(
        ctx.ParseResult.GetValueForOption(urlOption),
        ctx.ParseResult.GetValueForOption(keyOption));

// ---- profile ----
var profileCmd = new Command("profile", "Manage profiles and folders.");
profileCmd.AddCommand(ProfileCommands.ListProfiles(BuildClient));
profileCmd.AddCommand(ProfileCommands.GetProfile(BuildClient));
profileCmd.AddCommand(ProfileCommands.CreateProfile(BuildClient));
profileCmd.AddCommand(ProfileCommands.DeleteProfile(BuildClient));
profileCmd.AddCommand(ProfileCommands.ListFolders(BuildClient));
profileCmd.AddCommand(ProfileCommands.CreateFolder(BuildClient));
profileCmd.AddCommand(ProfileCommands.DeleteFolder(BuildClient));
root.AddCommand(profileCmd);

// ---- button ----
var buttonCmd = new Command("button", "Manage action buttons inside profile folders.");
buttonCmd.AddCommand(ButtonCommands.ListButtons(BuildClient));
buttonCmd.AddCommand(ButtonCommands.CreateButton(BuildClient));
buttonCmd.AddCommand(ButtonCommands.DeleteButton(BuildClient));
root.AddCommand(buttonCmd);

// ---- plugin ----
var pluginCmd = new Command("plugin", "Manage installed plugins and the Extension Store.");
pluginCmd.AddCommand(PluginCommands.ListPlugins(BuildClient));
pluginCmd.AddCommand(PluginCommands.GetPlugin(BuildClient));
pluginCmd.AddCommand(PluginCommands.SearchStore(BuildClient));
pluginCmd.AddCommand(PluginCommands.InstallPlugin(BuildClient));
root.AddCommand(pluginCmd);

// ---- variable ----
var varCmd = new Command("variable", "Manage MacroDeck variables.");
varCmd.AddCommand(VariableCommands.ListVariables(BuildClient));
varCmd.AddCommand(VariableCommands.GetVariable(BuildClient));
varCmd.AddCommand(VariableCommands.SetVariable(BuildClient));
varCmd.AddCommand(VariableCommands.DeleteVariable(BuildClient));
root.AddCommand(varCmd);

// ---- device ----
var deviceCmd = new Command("device", "Manage connected devices.");
deviceCmd.AddCommand(DeviceCommands.ListDevices(BuildClient));
deviceCmd.AddCommand(DeviceCommands.AssignProfile(BuildClient));
deviceCmd.AddCommand(DeviceCommands.SetBlocked(BuildClient));
root.AddCommand(deviceCmd);

// ---- config ----
var configCmd = new Command("config", "Read or update MacroDeck server configuration.");
configCmd.AddCommand(ConfigCommands.GetConfig(BuildClient));
configCmd.AddCommand(ConfigCommands.UpdateConfig(BuildClient));
root.AddCommand(configCmd);

return await root.InvokeAsync(args);
