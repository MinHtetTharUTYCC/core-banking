using MediatR;
using CoreBanking.Application.Auth.DTOs;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly IAccountRepository _accountRepository;

    public RegisterCommandHandler(
        IIdentityService identityService,
        ITokenService tokenService,
        IAccountRepository accountRepository)
    {
        _identityService = identityService;
        _tokenService = tokenService;
        _accountRepository = accountRepository;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var (success, userId, errors) = await _identityService.RegisterAsync(
            request.Email, request.Password, request.FullName, "Customer");

        if (!success)
            throw new InvalidOperationException(string.Join(", ", errors));

        var account = Account.Create(request.FullName, request.Email, AccountType.Savings, Currency.USD);
        account.Activate();
        await _accountRepository.AddAsync(account, cancellationToken);

        var roles = await _identityService.GetRolesAsync(userId);
        var accessToken = _tokenService.GenerateAccessToken(userId, request.Email, request.FullName, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _identityService.StoreRefreshTokenAsync(userId, refreshToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }
}
