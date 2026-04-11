namespace MacroDeck.Server.Dto;

public class ConfigDto
{
    public bool AutoStart { get; set; }
    public bool AutoUpdates { get; set; }
    public bool UpdateBetaVersions { get; set; }
    public bool EnableAdbServer { get; set; }
    public bool EnableAdbAutoStartApp { get; set; }
    public bool EnableSsl { get; set; }
    public string? SslCertificatePath { get; set; }
    public string HostAddress { get; set; } = string.Empty;
    public int HostPort { get; set; }
    public bool AskOnNewConnections { get; set; }
    public bool BlockNewConnections { get; set; }
    public string Language { get; set; } = string.Empty;
}
