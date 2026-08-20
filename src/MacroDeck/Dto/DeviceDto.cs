namespace MacroDeck.Server.Dto;

public class DeviceConfigDto
{
    public float Brightness { get; set; }
    public bool AutoConnect { get; set; }
    public string WakeLockMethod { get; set; } = string.Empty;
}

public class DeviceDto
{
    public string ClientId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public bool Blocked { get; set; }
    public bool Available { get; set; }
    public DeviceConfigDto? Configuration { get; set; }
}
