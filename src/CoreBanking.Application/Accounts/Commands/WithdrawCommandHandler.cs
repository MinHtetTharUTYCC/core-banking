using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Accounts.Commands;

public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, Unit>
{
    private readonly IAccountRepository _repository;

    public WithdrawCommandHandler(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(WithdrawCommand request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByIdAsync(request.AccountId);

        if(account is null)
            throw new KeyNotFoundException("Account not found.");

        if (account.OwnerEmail != request.OwnerEmail)
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");

        account.Withdraw(request.Amount);
        await _repository.UpdateAsync(account);

        return Unit.Value;
    }
}
