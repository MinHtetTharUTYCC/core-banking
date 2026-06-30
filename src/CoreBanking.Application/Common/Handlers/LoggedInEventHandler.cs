using System.Text.Json;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Models;
using CoreBanking.Domain.Enums;
using CoreBanking.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoreBanking.Application.Common.Handlers;

public class LoggedInEventHandler(IEmailService emailService,ILogger<LoggedInEventHandler> logger): INotificationHandler<LoggedInEvent>
{
    public async Task Handle(LoggedInEvent ev, CancellationToken cancellationToken)
    {
        try
        {
            await emailService.SendAndTrackAsync(
                ev.Email,
                ev.FullName,
                "LoggedInEmail",
                new LoggedInEmailModel
                {
                    FullName = ev.FullName,
                    Email = ev.Email,
                    Device = ev.Device,
                    Browser = ev.UserAgent,
                    OperatingSystem = ev.OperatingSystem,
                    IpAddress = ev.IpAddress,
                    LoginDateTime = ev.LoggedInAt
                },
                "Account Logged In!",
                NotificationType.LoggedIn,
                metadata: JsonSerializer.Serialize(new { UserId = ev.UserId }),
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send logged in email to {Email}", ev.Email);
        }
    }
}