using MediatR;
using CoreBanking.Application.Auth.DTOs;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Auth.Commands.Login;

public class LoginCommandHandler(IIdentityService identityService,
    ITokenService tokenService) : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var (success, user, errors) = await identityService.ValidateCredentialsAsync(request.Email, request.Password);

        if (!success || user == null)
            throw new InvalidOperationException("Invalid email or password");

        var roles = await identityService.GetRolesAsync(user.Id);
        var accessToken = tokenService.GenerateAccessToken(user.Id, user.Email,user.FullName, roles);
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
