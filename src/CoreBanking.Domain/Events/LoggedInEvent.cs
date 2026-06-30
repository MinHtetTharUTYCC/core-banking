using CoreBanking.Domain.Common;

namespace CoreBanking.Domain.Events;

public sealed class LoggedInEvent(
    string userId,
    string email,
    string fullName,
    string ipAddress,
    string userAgent,
    string operatingSystem,
    string device,
    DateTime loggedInAt): DomainEvent
{
    public string UserId { get; } = userId;
    public string Email { get; } = email;
    public string FullName { get; } = fullName;
    
    public string IpAddress { get; } = ipAddress;
    public string UserAgent { get; } = userAgent;
    public string OperatingSystem { get; } = operatingSystem;
    public string Device { get; } = device;
    
    public DateTime LoggedInAt { get; } = loggedInAt;
    
}