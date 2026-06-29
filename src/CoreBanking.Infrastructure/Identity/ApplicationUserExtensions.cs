using CoreBanking.Application.Common.Models;

namespace CoreBanking.Infrastructure.Identity;

public static class ApplicationUserExtensions
{
    public static UserDto ToUserDto(this ApplicationUser user) => new()
    {
        Id = user.Id,
        Email = user.Email ?? throw new InvalidOperationException("User has no email."),
        FullName = user.FullName
    };
}