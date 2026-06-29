namespace CoreBanking.Infrastructure.Identity;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public DateTime Expires { get; init; }
    public DateTime Created { get; init; } = DateTime.UtcNow;
    public bool IsExpired => DateTime.UtcNow >= Expires;
}
