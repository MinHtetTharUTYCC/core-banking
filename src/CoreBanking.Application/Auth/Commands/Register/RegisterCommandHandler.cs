using MediatR;
using CoreBanking.Application.Auth.DTOs;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Application.Auth.Commands.Register;

public class RegisterCommandHandler(IIdentityService identityService,
    ITokenService tokenService,
    IAccountRepository accountRepository) : IRequestHandler<RegisterCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var (success, userId, errors) = await identityService.RegisterAsync(
            request.Email, request.Password, request.FullName, "Customer");

        if (!success)
            throw new InvalidOperationException(string.Join(", ", errors));

        var account = Account.Create(request.FullName, request.Email, AccountType.Savings, Currency.USD);
        account.Activate();
        await accountRepository.AddAsync(account, cancellationToken);

        var roles = await identityService.GetRolesAsync(userId);
        var accessToken = tokenService.GenerateAccessToken(userId, request.Email, request.FullName, roles);
        var refreshToken = tokenService.GenerateRefreshToken();

        await identityService.StoreRefreshTokenAsync(userId, refreshToken,cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }
}
