using MediatR;
using CoreBanking.Application.Auth.DTOs;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler(IIdentityService identityService,
    ITokenService tokenService) : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await identityService.ValidateRefreshTokenAsync(request.RefreshToken);
        if (user == null)
            throw new InvalidOperationException("Invalid refresh token");

        var roles = await identityService.GetRolesAsync(user.Id);
        var accessToken = tokenService.GenerateAccessToken(user.Id, user.Email!, user.FullName, roles);
        var refreshToken = tokenService.GenerateRefreshToken();

        await identityService.StoreRefreshTokenAsync(user.Id, refreshToken,cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }
}
