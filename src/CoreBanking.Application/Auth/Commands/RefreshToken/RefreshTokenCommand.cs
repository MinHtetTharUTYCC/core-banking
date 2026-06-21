using MediatR;
using CoreBanking.Application.Auth.DTOs;

namespace CoreBanking.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand : IRequest<AuthResponse>
{
    public string RefreshToken { get; init; } = string.Empty;
}
