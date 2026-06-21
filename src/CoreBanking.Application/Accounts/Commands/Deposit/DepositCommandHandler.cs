using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Accounts.Commands;

namespace CoreBanking.Application.Accounts.Commands;

public class DepositCommandHandler : IRequestHandler<DepositCommand, Unit>
{
    private readonly IAccountRepository _repository;

    public DepositCommandHandler(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByIdAsync(request.AccountId);

        if(account is null)
            return new KeyNotFoundException("Account not found");

        if (account.OwnerEmail != request.OwnerEmail)
            return new UnauthorizedAccessException("You do not own this account");

        account.Deposit(request.Amount);
        await _repository.UpdateAsync(account);

        return Unit.Value;
    }
}
