using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Application.Transactions.Queries;

public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDto?>
{
    private readonly ITransactionRepository _repository;

    public GetTransactionByIdQueryHandler(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<TransactionDto?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _repository.GetByIdAsync(request.Id);
        if (transaction == null)
            return null;

        return MapToDto(transaction);
    }

    internal static TransactionDto MapToDto(Transaction t)
    {
        return new TransactionDto
        {
            Id = t.Id,
            AccountId = t.AccountId,
            Type = t.Type.ToString(),
            Amount = t.Amount,
            Currency = t.Currency.ToString(),
            BalanceBefore = t.BalanceBefore,
            BalanceAfter = t.BalanceAfter,
            Status = t.Status.ToString(),
            Description = t.Description,
            ReferenceNumber = t.ReferenceNumber,
            RelatedAccountId = t.RelatedAccountId,
            CreatedAt = t.CreatedAt
        };
    }
}
