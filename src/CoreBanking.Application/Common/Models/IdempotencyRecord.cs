using  CoreBanking.Domain.Enums;

namespace CoreBanking.Application.Common.Models;

public class IdempotencyRecord<TResponse>
{
    public IdempotencyStatus Status { get; set; }
    
    public TResponse? Response { get; init; }
    
    public DateTime ExecutedAt { get; init; }
    
}
