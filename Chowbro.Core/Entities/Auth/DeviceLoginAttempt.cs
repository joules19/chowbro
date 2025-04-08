namespace Chowbro.Core.Entities.Auth;

public class DeviceLoginAttempt
{
    public Guid Id { get; set; }
    public string DeviceId { get; set; }
    public DateTime AttemptDate { get; set; }
    public string? IpAddress { get; set; }
}