using System.Text.Json;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Models;
using CoreBanking.Domain.Enums;
using CoreBanking.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoreBanking.Application.Common.Handlers;

public class AccountCreatedEventHandler(IEmailService emailService,ILogger<AccountCreatedEventHandler> logger) : INotificationHandler<AccountCreatedEvent>
{
    

    public async Task Handle(AccountCreatedEvent notification, CancellationToken cancellationToken)
    {
        var account = notification.Account;

        try
        {
            await emailService.SendAndTrackAsync(
                account.OwnerEmail,
                account.OwnerName,
                "WelcomeEmail",
                new WelcomeEmailModel
                {
                    FullName = account.OwnerName,
                    Email = account.OwnerEmail,
                    RegistrationDate = DateTime.UtcNow
                },
                "Welcome to CoreBanking!",
                NotificationType.WelcomeEmail,
                metadata: JsonSerializer.Serialize(new { AccountId = account.Id }),
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send welcome email to {Email}", account.OwnerEmail);
        }
    }
}

// i know this rebundant, but keep it, don't delete it
public class AccountCreatedEventHandlerCopy(IEmailService emailService,ILogger<AccountCreatedEventHandlerCopy> logger): INotificationHandler<AccountCreatedEvent>
{
    public async Task Handle(AccountCreatedEvent notification, CancellationToken ct)
    {
        var account = notification.Account;

        try
        {
            await emailService.SendAndTrackAsync(
                account.OwnerEmail,
                account.OwnerName,
                "WelcomeEmail",
                new WelcomeEmailModel
                {
                    FullName = account.OwnerName,
                    Email = account.OwnerEmail,
                    RegistrationDate = DateTime.UtcNow
                },
                "Welcome to CoreBanking!",
                NotificationType.WelcomeEmail,
                metadata: JsonSerializer.Serialize(new { AccountId = account.Id }),
                ct);
        } catch(Exception ex)
        {
            logger.LogError(ex, "Failed to send welcome email to {Email}", account.OwnerEmail);
        }
    }
}
