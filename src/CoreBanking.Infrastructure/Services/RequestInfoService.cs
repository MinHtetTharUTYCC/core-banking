using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using UAParser;

namespace CoreBanking.Infrastructure.Services;

public class RequestInfoService(IHttpContextAccessor httpContextAccessor): IRequestInfoService
{
    public string GetIpAddress()
    {
        return httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    public DeviceInfo GetDeviceInfo()
    {
        var userAgent = httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString() ?? string.Empty;
        var client = Parser.GetDefault().Parse(userAgent);

        return new DeviceInfo(
            Browser: $"{client.UA.Family} {client.UA.Major}".Trim(),
            OperatingSystem: client.OS.Family,
            Device: client.Device.Family,
            UserAgent: userAgent
        );
    }
}