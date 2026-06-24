using CoreBanking.Application.Common.Extensions;

namespace CoreBanking.Application.Common.Handlers;

public class MoneyTransferredEventHanlder: IEventHandler<MoneyTransferredEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<MoenyTransferredEventHanlder> _logger;

    public MoneyTransferredEventHanlder(IEmailService emailService, ILogger<MoenyTransferredEventHanlder> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(MoneyTransferredEvent transfer,CancellationToken ct)
    {
        var sendEmailTask = SendSentNotificationAsync(transfer, ct);
        var receiveEmailTask = SendReceivedNotificationAsync(transfer, ct);

        await Task.WhenAll(sendEmailTask, receiveEmailTask);
    }

    private async Task SendSentNotificationAsync(MoneyTransferredEvent transfer, CancellationToken ct)
    {
        try
        {
            await _emailService.SendAndTrackAsync(
                fromAccount.OwnerEmail,
                fromAccount.OwnerName,
                "MoneyTranserredEmail",
                new TransactionSentEmailModel
                {
                    Email = transfer.FromAccount.OwnerEmail,
                    FullName = transfer.FromAccount.OwnerName,
                    Amount = amount,
                    Currency = transfer.FromAccount.Currency,
                    TransactionDate = transfer.DateTime,
                    NewBalance = transfer.FromAccount.Balance,
                    RecipientEmail = transfer.ToAccount.RecipientEmail,
                    RecipientAccountNumberMasked = transfer.ToAccount.AccountNumber.MaskAccountNumber(),
                },
                "Money Transfer",
                null,
                ct,
            )
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"{transfer.OccurredOn} Failed to send money transfer {transfer.EventId} notification email: from {transfer.FromAccount.OwnerEmail} to {transfer.ToAccount.OwnerEmail}");
        }
    }
    private async Task SendReceivedNotificationAsync(MoneyTransferredEvent transfer, CancellationToken ct)
    {
        try
        {
            await _emailService.SendAndTrackAsync(
                toAccount.OwnerEmail,
                toAccount.OwnerName,
                "MoneyReceivedEmail",
                new TransactionReceivedEmailModel
                {
                    Email = transfer.ToAccount.OwnerEmail,
                    FullName = transfer.ToAccount.OwnerName,
                    Amount = transfer.Amount,
                    Currency = transfer.ToAccount.Currency,
                    TransactionDate = transfer.DateTime,
                    NewBalance = transfer.ToAccount.Balance,
                    SenderEmail = transfer.FromAccount.SenderEmail,
                    SenderAccountNumberMasked = transfer.FromAccount.AccountNumber.MaskAccountNumber(),
                },
                "Money Received",
                null,
                ct,
             );

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{transfer.OccurredOn} Failed to send money received {transfer.EventId} notification email: from {transfer.FromAccount.OwnerEmail} to {transfer.ToAccount.OwnerEmail}");
        }
    }
}