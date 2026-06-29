using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Common.Models;

namespace CoreBanking.Infrastructure.Identity;

public class TokenService(IConfiguration configuration,ApplicationDbContext context) : ITokenService
{
    public string GenerateAccessToken(string userId, string email, string fullName, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email!),
            new("full_name", fullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(configuration["Jwt:AccessTokenExpirationMinutes"])),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public async Task StoreRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        var storedToken = new RefreshToken
        {
            Token = refreshToken,
            UserId = userId,
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow
        };

        context.RefreshTokens.Add(storedToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserDto?> ValidateRefreshTokenAsync(string refreshToken)
    {
        var storedToken = await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsExpired);

        if (storedToken == null)
            return null;

        var user = storedToken.User;
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? throw new InvalidOperationException("User has no email."),
            FullName = user.FullName
        };
    }
}
