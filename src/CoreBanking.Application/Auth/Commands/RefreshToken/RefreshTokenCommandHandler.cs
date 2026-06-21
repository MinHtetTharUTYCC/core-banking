using MediatR;
using CoreBanking.Application.Auth.DTOs;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(IIdentityService identityService, ITokenService tokenService)
    {
        _identityService = identityService;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.ValidateRefreshTokenAsync(request.RefreshToken);
        if (user == null)
            throw new InvalidOperationException("Invalid refresh token");

        var roles = await _identityService.GetRolesAsync(user.Id);
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, user.FullName, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _identityService.StoreRefreshTokenAsync(user.Id, refreshToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }
}
