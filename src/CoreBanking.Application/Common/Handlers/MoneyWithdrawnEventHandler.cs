using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Models;
using CoreBanking.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoreBanking.Application.Common.Handlers;

public class MoneyWithdrawnEventHandler : INotificationHandler<MoneyWithdrawnEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<MoneyWithdrawnEventHandler> _logger;

    public MoneyWithdrawnEventHandler(IEmailService emailService, ILogger<MoneyWithdrawnEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(MoneyWithdrawnEvent notification, CancellationToken cancellationToken)
    {
        var account = notification.Account;

        try
        {
            await _emailService.SendAndTrackAsync(
                account.OwnerEmail,
                account.OwnerName,
                "Withdraw",
                new WithdrawEmailModel
                {
                    FullName = account.OwnerName,
                    Email = account.OwnerEmail,
                    Amount = notification.Amount,
                    Currency = account.Balance.Currency.ToString(),
                    TransactionDate = DateTime.UtcNow,
                    NewBalance = account.Balance.Amount
                },
                "Withdrawal Confirmed",
                Domain.Enums.NotificationType.TransactionAlert,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send withdrawal email to {Email}", account.OwnerEmail);
        }
    }
}
