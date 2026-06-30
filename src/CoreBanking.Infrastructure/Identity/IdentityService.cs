using Microsoft.AspNetCore.Identity;
using CoreBanking.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using CoreBanking.Application.Common.Models;

namespace CoreBanking.Infrastructure.Identity;

public class IdentityService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    : IIdentityService
{
    public async Task<(bool Success, string UserId, string[] Errors)> RegisterAsync(string email, string password, string fullName, string role)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return (false, string.Empty, result.Errors.Select(e => e.Description).ToArray());

        await userManager.AddToRoleAsync(user, role);
        return (true, user.Id, []);
    }

    public async Task<(bool Success, UserDto? User, string[] Errors)> ValidateCredentialsAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        
        if (user == null || !await userManager.CheckPasswordAsync(user, password))
            return (false, null, ["Invalid email or password"]);

        return (true, user.ToUserDto(), []);
    }

    public async Task<UserDto?> FindByIdAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        return user?.ToUserDto();
    }

    public async Task<IList<string>> GetRolesAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        if(user is null)
            return new List<string>();

        return await userManager.GetRolesAsync(user);
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
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.Expires > DateTime.UtcNow);

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
