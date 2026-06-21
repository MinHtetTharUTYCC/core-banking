using MediatR;
using CoreBanking.Application.Auth.DTOs;

namespace CoreBanking.Application.Auth.Commands.Register;

public record RegisterCommand : IRequest<AuthResponse>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
}