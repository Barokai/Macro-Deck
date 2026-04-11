using System.Net.Http.Headers;
using MacroDeck.Mcp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// The MCP server communicates over stdio (stdin/stdout) so that any MCP-compatible
// client (Claude Desktop, VS Code Copilot, etc.) can launch it as a subprocess.
// Configure the MacroDeck server URL and admin key via environment variables:
//   MACRODECK_URL      (default: http://localhost:8191)
//   MACRODECK_API_KEY  (found in MacroDeck's config.json as "AdminApiKey")

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(opts => opts.LogToStandardErrorThreshold = LogLevel.Trace);

// HTTP client for MacroDeck admin REST API
builder.Services.AddHttpClient<MacroDeckApiClient>(client =>
{
    var url = Environment.GetEnvironmentVariable("MACRODECK_URL") ?? "http://localhost:8191";
    var key = Environment.GetEnvironmentVariable("MACRODECK_API_KEY") ?? string.Empty;
    client.BaseAddress = new Uri(url.TrimEnd('/') + "/");
    client.DefaultRequestHeaders.Add("X-MacroDeck-Admin-Key", key);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

// Register all MCP tools
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly(typeof(Program).Assembly);

await builder.Build().RunAsync();
