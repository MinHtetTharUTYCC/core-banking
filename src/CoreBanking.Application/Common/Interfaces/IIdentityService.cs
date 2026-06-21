using CoreBanking.Application.Common.Models;

namespace CoreBanking.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Success, string UserId, string[] Errors)> RegisterAsync(string email, string password, string fullName, string role);
    Task<(bool Success, UserDto? User, string[] Errors)> ValidateCredentialsAsync(string email, string password);
    Task<UserDto?> FindByIdAsync(string userId);
    Task<IList<string>> GetRolesAsync(string userId);
    Task StoreRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<UserDto?> ValidateRefreshTokenAsync(string refreshToken);
}
