using MacroDeck.Server.Dto;
using MacroDeck.Server.Dto.Requests;
using MacroDeck.Server.Services;
using SuchByte.MacroDeck.StartupConfig;

namespace SuchByte.MacroDeck.Server.AdminServices;

public class ConfigAdminService : IConfigAdminService
{
    public ConfigDto GetConfig()
    {
        var cfg = MacroDeck.Configuration;
        return new ConfigDto
        {
            AutoStart = cfg.AutoStart,
            AutoUpdates = cfg.AutoUpdates,
            UpdateBetaVersions = cfg.UpdateBetaVersions,
            EnableAdbServer = cfg.EnableAdbServer,
            EnableAdbAutoStartApp = cfg.EnableAdbAutoStartApp,
            EnableSsl = cfg.EnableSsl,
            SslCertificatePath = cfg.SslCertificatePem,
            HostAddress = cfg.HostAddress,
            HostPort = cfg.HostPort,
            AskOnNewConnections = cfg.AskOnNewConnections,
            BlockNewConnections = cfg.BlockNewConnections,
            Language = cfg.Language,
        };
    }

    public ConfigDto UpdateConfig(UpdateConfigRequest request)
    {
        var cfg = MacroDeck.Configuration;

        if (request.AutoStart.HasValue) cfg.AutoStart = request.AutoStart.Value;
        if (request.AutoUpdates.HasValue) cfg.AutoUpdates = request.AutoUpdates.Value;
        if (request.UpdateBetaVersions.HasValue) cfg.UpdateBetaVersions = request.UpdateBetaVersions.Value;
        if (request.EnableAdbServer.HasValue) cfg.EnableAdbServer = request.EnableAdbServer.Value;
        if (request.EnableAdbAutoStartApp.HasValue) cfg.EnableAdbAutoStartApp = request.EnableAdbAutoStartApp.Value;
        if (request.AskOnNewConnections.HasValue) cfg.AskOnNewConnections = request.AskOnNewConnections.Value;
        if (request.BlockNewConnections.HasValue) cfg.BlockNewConnections = request.BlockNewConnections.Value;
        if (request.Language is not null) cfg.Language = request.Language;

        cfg.Save(ApplicationPaths.MainConfigFilePath);

        return GetConfig();
    }

    public string GetAdminApiKey() => MacroDeck.Configuration.AdminApiKey;
}
