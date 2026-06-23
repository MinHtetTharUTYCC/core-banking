namespace CoreBanking.Domain.Entities;

public class IdempotencyKey
{
    public Guid Id { get; private set; }
    public string Key { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public string? Response { get; private set; }

    private IdempotencyKey() { }

    public static IdempotencyKey Create(string key, TimeSpan? ttl = null)
    {
        var duration = ttl ?? TimeSpan.FromHours(24);
        return new IdempotencyKey
        {
            Id = Guid.NewGuid(),
            Key = key,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(duration)
        };
    }

    public void SetResponse(string response)
    {
        Response = response;
    }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
}
