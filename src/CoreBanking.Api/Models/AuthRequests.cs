using System.ComponentModel.DataAnnotations;

namespace CoreBanking.Api.Models;

public record RegisterRequest(
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)][MaxLength(32)] string Password,
    [Required][MinLength(2)][MaxLength(32)] string FullName
);

public record LoginRequest(
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)][MaxLength(32)] string Password
);

public record RefreshTokenRequest(
    [Required] string RefreshToken
);
