using MediatR;
using CoreBanking.Application.Auth.DTOs;

namespace CoreBanking.Application.Auth.Commands.Login;

public record LoginCommand : IRequest<AuthResponse>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
