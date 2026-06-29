using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(IDatabaseTransaction transaction)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        await transaction.BeginAsync(cancellationToken);

        try
        {
            var response = await next(cancellationToken);

            await transaction.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return response;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
