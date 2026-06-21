using CoreBanking.Application.Common.Models;

namespace CoreBanking.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(string userId, string email, string fullName, IList<string> roles);
    string GenerateRefreshToken();
    Task StoreRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<UserDto?> ValidateRefreshTokenAsync(string refreshToken);
}
