using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Accounts.Commands;

public class TransferCommandHandler : IRequestHandler<TransferCommand, Unit>
{
    private readonly IAccountRepository _repository;

    public TransferCommandHandler(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        var fromAccount = await _repository.GetByIdAsync(request.FromAccountId);


        if (fromAccount == null)
            throw new KeyNotFoundException("Accont not found");

        if(fromAccount.OwnerEmail != request.OwnerEmail)
            throw new BadRequestException("You are not authorized to transfer from this account");

        var toAccount = await _repository.GetByIdAsync(request.ToAccountId);
        if (toAccount == null)
            throw new KeyNotFoundException("Account to transfer to not found");

        fromAccount.Transfer(toAccount, request.Amount);

        await _repository.UpdateAsync(fromAccount);
        await _repository.UpdateAsync(toAccount);

        return Unit.Value;
    }
}
