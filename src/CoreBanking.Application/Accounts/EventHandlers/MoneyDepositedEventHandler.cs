using MediatR;
using CoreBanking.Domain.Events;

namespace CoreBanking.Application.Accounts.EventHandlers;

public class MoneyDepositedEventHandler : INotificationHandler<MoneyDepositedEvent>
{
    // private readonly IEmailService _emailService;
    // private readonly ILogger<MoneyDepositedEventHandler> _logger;

    // public MoneyDepositedEventHandler(IEmailService emailService, ILogger<MoneyDepositedEventHandler> logger)
    // {
    //     _emailService = emailService;
    //     _logger = logger;
    // }

    public async Task Handle(MoneyDepositedEvent notification, CancellationToken cancellationToken)
    {
        // TODO: Implement email notification when MoneyDeposited event occurs

        // var account = notification.Account;
        // var amount = notification.Amount;

        // _logger.LogInformation(
        //     "Money deposited: Account {AccountId}, Amount {Amount}",
        //     account.Id, amount);

        // await _emailService.SendAsync(
        //     account.OwnerEmail,
        //     "Deposit Confirmation",
        //     $"Dear {account.OwnerName},\n\n" +
        //     $"A deposit of {amount:C} has been made to your account {account.AccountNumber.Value}.\n" +
        //     $"New balance: {account.Balance.Amount:C}\n\n" +
        //     $"Thank you for banking with us!");

        await Task.CompletedTask;
    }
}
