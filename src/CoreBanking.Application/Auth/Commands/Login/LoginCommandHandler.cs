using MediatR;
using CoreBanking.Application.Auth.DTOs;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IIdentityService identityService, ITokenService tokenService)
    {
        _identityService = identityService;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var (success, user, errors) = await _identityService.ValidateCredentialsAsync(request.Email, request.Password);

        if (!success || user == null)
            throw new InvalidOperationException("Invalid email or password");

        var roles = await _identityService.GetRolesAsync(user.Id);
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email,user.FullName, roles);
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
