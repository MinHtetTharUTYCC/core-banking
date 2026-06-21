using MediatR;

namespace CoreBanking.Application.Transactions.Queries;

public record GetTransactionByIdQuery : IRequest<TransactionDto>
{
    public Guid Id { get; init; }
}
