namespace CoreBanking.Application.Common.Models;

public sealed record DeviceInfo(
    string Browser,
    string OperatingSystem,
    string Device,
    string UserAgent
);