using CoreBanking.Application.Common.Extensions;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Models;
using CoreBanking.Domain.Enums;
using CoreBanking.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoreBanking.Application.Common.Handlers;

public class MoneyTransferredEventHandler(IEmailService emailService,
 ILogger<MoneyTransferredEventHandler> logger) : INotificationHandler<MoneyTransferredEvent>
{

    public async Task Handle(MoneyTransferredEvent transfer, CancellationToken ct)
    {
        var sendEmailTask = SendSentNotificationAsync(transfer, ct);
        var receiveEmailTask = SendReceivedNotificationAsync(transfer, ct);

        await Task.WhenAll(sendEmailTask, receiveEmailTask);
    }

    private async Task SendSentNotificationAsync(MoneyTransferredEvent transfer, CancellationToken ct)
    {
        try
        {
            await emailService.SendAndTrackAsync(
                transfer.FromAccount.OwnerEmail,
                transfer.FromAccount.OwnerName,
                "MoneyTransferredEmail",
                new TransactionSentEmailModel
                {
                    Email = transfer.FromAccount.OwnerEmail,
                    FullName = transfer.FromAccount.OwnerName,
                    Amount = transfer.Amount,
                    Currency = transfer.FromAccount.Balance.Currency.ToString(),
                    TransactionDate = transfer.OccurredOn,
                    NewBalance = transfer.FromAccount.Balance.Amount,
                    RecipientName = transfer.ToAccount.OwnerName,
                    RecipientAccountNumberMasked = transfer.ToAccount.AccountNumber.Value.MaskAccountNumber(),
                    BankName = "CoreBanking",
                },
                "Money Transfer Sent",
                NotificationType.TransactionAlert,
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send money transfer notification from {Email}", transfer.FromAccount.OwnerEmail);
        }
    }

    private async Task SendReceivedNotificationAsync(MoneyTransferredEvent transfer, CancellationToken ct)
    {
        try
        {
            await emailService.SendAndTrackAsync(
                transfer.ToAccount.OwnerEmail,
                transfer.ToAccount.OwnerName,
                "MoneyReceivedEmail",
                new TransactionReceivedEmailModel
                {
                    Email = transfer.ToAccount.OwnerEmail,
                    FullName = transfer.ToAccount.OwnerName,
                    Amount = transfer.Amount,
                    Currency = transfer.ToAccount.Balance.Currency.ToString(),
                    TransactionDate = transfer.OccurredOn,
                    NewBalance = transfer.ToAccount.Balance.Amount,
                    SenderName = transfer.FromAccount.OwnerName,
                    SenderAccountNumberMasked = transfer.FromAccount.AccountNumber.Value.MaskAccountNumber(),
                    BankName = "CoreBanking",
                },
                "Money Received",
                NotificationType.TransactionAlert,
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send money received notification to {Email}", transfer.ToAccount.OwnerEmail);
        }
    }
}
