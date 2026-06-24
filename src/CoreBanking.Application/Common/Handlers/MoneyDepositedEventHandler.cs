using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Models;
using CoreBanking.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoreBanking.Application.Common.Handlers;

public class MoneyDepositedEventHandler : INotificationHandler<MoneyDepositedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<MoneyDepositedEventHandler> _logger;

    public MoneyDepositedEventHandler(IEmailService emailService, ILogger<MoneyDepositedEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(MoneyDepositedEvent notification, CancellationToken cancellationToken)
    {
        var account = notification.Account;

        try
        {
            await _emailService.SendAndTrackAsync(
                account.OwnerEmail,
                account.OwnerName,
                "Deposit",
                new DepositEmailModel
                {
                    FullName = account.OwnerName,
                    Email = account.OwnerEmail,
                    Amount = notification.Amount,
                    Currency = account.Balance.Currency.ToString(),
                    TransactionDate = DateTime.UtcNow,
                    NewBalance = account.Balance.Amount
                },
                "Deposit Confirmed",
                Domain.Enums.NotificationType.TransactionAlert,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send deposit email to {Email}", account.OwnerEmail);
        }
    }
}
