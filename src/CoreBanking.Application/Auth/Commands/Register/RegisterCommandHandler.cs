using MediatR;
using CoreBanking.Application.Auth.DTOs;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;

    public RegisterCommandHandler(IIdentityService identityService, ITokenService tokenService)
    {
        _identityService = identityService;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var (success, userId, errors) = await _identityService.RegisterAsync(
            request.Email, request.Password, request.FullName, "Customer");

        if (!success)
            throw new InvalidOperationException(string.Join(", ", errors));

        var roles = await _identityService.GetRolesAsync(userId);
        var accessToken = _tokenService.GenerateAccessToken(userId,request.Email, request.FullName, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _identityService.StoreRefreshTokenAsync(user!, refreshToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }
}
