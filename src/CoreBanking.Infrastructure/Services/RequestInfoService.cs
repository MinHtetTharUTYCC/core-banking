using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using UAParser;

namespace CoreBanking.Infrastructure.Services;

public class RequestInfoService(IHttpContextAccessor httpContextAccessor): IRequestInfoService
{
    public string GetIpAddress()
    {
        var ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        return ip is "::1" or "127.0.0.1" ? "localhost" : ip;
    }

    public DeviceInfo GetDeviceInfo()
    {
        var userAgent = httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString() ?? string.Empty;
        var client = Parser.GetDefault().Parse(userAgent);

        var device = client.Device.Family;
        if (device is "Other" or "other" or "Desktop" or "desktop")
        {
            var os = client.OS.Family.ToLowerInvariant();
            device = os switch
            {
                var o when o.Contains("windows") || o.Contains("mac") || o.Contains("linux") || o.Contains("chrome os") => "Desktop",
                var o when o.Contains("android") => "Mobile",
                var o when o.Contains("ios") || o.Contains("iphone") || o.Contains("ipad") => client.OS.Family.Contains("pad") ? "Tablet" : "Mobile",
                _ => "Desktop"
            };
        }

        return new DeviceInfo(
            Browser: $"{client.UA.Family} {client.UA.Major}".Trim(),
            OperatingSystem: client.OS.Family,
            Device: device,
            UserAgent: userAgent
        );
    }
}