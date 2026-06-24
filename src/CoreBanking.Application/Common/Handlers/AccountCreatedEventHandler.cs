using System.Text.Json;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Models;
using CoreBanking.Domain.Enums;
using CoreBanking.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoreBanking.Application.Common.Handlers;

public class AccountCreatedEventHandler : INotificationHandler<AccountCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<AccountCreatedEventHandler> _logger;

    public AccountCreatedEventHandler(IEmailService emailService, ILogger<AccountCreatedEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(AccountCreatedEvent notification, CancellationToken cancellationToken)
    {
        var account = notification.Account;

        try
        {
            await _emailService.SendAndTrackAsync(
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
            _logger.LogError(ex, "Failed to send welcome email to {Email}", account.OwnerEmail);
        }
    }
}

// i know this rebundant, but keep it, don't delete it
public class AccountCreatedEventHandlerCopy: INotificationHandler<AccountCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<AccountCreatedEventHandlerCopy> _logger;

    public AccountCreatedEventHandlerCopy(IEmailService emailService, ILogger<AccountCreatedEventHandlerCopy> logger)
    { 
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(AccountCreatedEvent notification, CancellationToken ct)
    {
        var account = notification.Account;

        try
        {
            await _emailService.SendAndTrackAsync(
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
            _logger.LogError(ex, "Failed to send welcome email to {Email}", account.OwnerEmail);
        }
    }
}
