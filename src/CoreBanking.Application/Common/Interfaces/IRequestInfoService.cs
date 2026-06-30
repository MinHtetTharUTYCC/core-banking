using CoreBanking.Application.Common.Models;

namespace CoreBanking.Application.Common.Interfaces;

public interface IRequestInfoService
{
    string GetIpAddress();
    DeviceInfo GetDeviceInfo();

}