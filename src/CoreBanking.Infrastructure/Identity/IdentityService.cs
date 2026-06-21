using Microsoft.AspNetCore.Identity;
using CoreBanking.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using CoreBanking.Application.Common.Models;

namespace CoreBanking.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public IdentityService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

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

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return (false, string.Empty, result.Errors.Select(e => e.Description).ToArray());

        await _userManager.AddToRoleAsync(user, role);
        return (true, user.Id, []);
    }

    public async Task<(bool Success, UserDto? User, string[] Errors)> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return (false, null, ["Invalid email or password"]);

        var validPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!validPassword)
            return (false, null, ["Invalid email or password"]);

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName
        };

        return (true, userDto, Array.Empty<string>());
    }

    public async Task<UserDto?> FindByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName
        };
    }

    public async Task<IList<string>> GetRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if(user is null)
            return new List<string>();

        return await _userManager.GetRolesAsync(user);
    }

    public async Task StoreRefreshTokenAsync(string userId, string refreshToken)
    {
        var storedToken = new RefreshToken
        {
            Token = refreshToken,
            UserId = userId,
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(storedToken);
        await _context.SaveChangesAsync();
    }

    public async Task<UserDto?> ValidateRefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsExpired);

        if (storedToken == null)
            return null;

        var user = storedToken.User;
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName
        };
    }
}
