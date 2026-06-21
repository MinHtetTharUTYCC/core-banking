namespace CoreBanking.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(object user, IList<string> roles);
    string GenerateRefreshToken();
    Task StoreRefreshTokenAsync(object user, string refreshToken);
    Task<object?> ValidateRefreshTokenAsync(string refreshToken);
}
