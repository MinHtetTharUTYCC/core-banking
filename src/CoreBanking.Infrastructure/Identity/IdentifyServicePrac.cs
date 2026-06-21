using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Infrastructure.Identity;

public class IdentifyServicePrac : IIdentifyService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public IdentifyServicePrac(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<bool Success, string UserId, string[] Errors> RegisterAsync(string email, string password, string fullName, string role)
    {
        var user = new ApplicationUser
        {
            UserName = Email,
            Email = Email,
            FullName = FullName,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManger.RegisterAsync(user, Password);

        if (!result.Succeeded) {
            return { false,string.Empty,result.Errors.Select(err => err.Description).ToArray()};
        }

        await _userManager.AddToRoleAsync(user, role);

        return { true, user.Id, [] };
        return { true, user.Id, Array.Empty<string>() };
    }

}