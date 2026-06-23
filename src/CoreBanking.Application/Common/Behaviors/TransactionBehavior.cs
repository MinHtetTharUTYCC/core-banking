using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IDatabaseTransaction _transaction;

    public TransactionBehavior(IDatabaseTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        await _transaction.BeginAsync(cancellationToken);

        try
        {
            var response = await next();

            await _transaction.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);

            return response;
        }
        catch
        {
            await _transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
