using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuchByte.MacroDeck.StartupConfig;

namespace SuchByte.MacroDeck;

public static class MacroDeckServerHelper
{
    private static IHost? _host;

    internal static bool UseHttps { get; private set; }

    public static async Task Setup(int port, X509Certificate2? certificate,
        Action<IServiceCollection> configureAdminServices)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
        }

        UseHttps = certificate is not null;

        _host = Host.CreateDefaultBuilder()
            .ConfigureSerilog()
            .ConfigureWebHostDefaults(hostBuilder =>
            {
                hostBuilder.UseStartup<ServerStartup>();
                hostBuilder.ConfigureKestrel(options =>
                {
                    if (UseHttps)
                    {
                        options.ListenAnyIP(port,
                            listenOptions =>
                            {
                                listenOptions.UseHttps(certificate!);
                                listenOptions.Protocols = HttpProtocols.Http1;
                            });
                    }
                    else
                    {
                        options.ListenAnyIP(port);
                    }
                });
            })
            .ConfigureServices(services =>
            {
                configureAdminServices(services);
            })
            .Build();

        await _host.RunAsync();
    }
}
