using System.Text.Json;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Enums;
using CoreBanking.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using RazorLight;

namespace CoreBanking.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<EmailService> _logger;
    private readonly RazorLightEngine _razorEngine;

    public EmailService(
        IOptions<EmailSettings> settings,
        INotificationRepository notificationRepository,
        ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _notificationRepository = notificationRepository;
        _logger = logger;

        _razorEngine = new RazorLightEngineBuilder()
            .UseFileSystemProject(Path.Combine(AppContext.BaseDirectory, "EmailTemplates"))
            .UseMemoryCachingProvider()
            .Build();
    }

    public async Task<bool> SendAsync(
        string toEmail, string toName, string subject, string htmlBody,
        CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled)
        {
            _logger.LogWarning("Email sending is disabled in configuration");
            return false;
        }

        try
        {
            var message = CreateMimeMessage(toEmail, toName, subject, htmlBody);
            await SendViaSmtpAsync(message, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendTemplatedAsync<TModel>(
        string toEmail, string toName, string templateKey, TModel model,
        string subject, CancellationToken cancellationToken = default)
    {
        var htmlBody = await _razorEngine.CompileRenderAsync(templateKey, model);
        return await SendAsync(toEmail, toName, subject, htmlBody, cancellationToken);
    }

    public async Task<bool> SendAndTrackAsync<TModel>(
        string toEmail, string toName, string templateKey, TModel model,
        string subject, NotificationType notificationType,
        string? metadata = null, CancellationToken cancellationToken = default)
    {
        var htmlBody = await _razorEngine.CompileRenderAsync(templateKey, model);

        var notification = Notification.Create(
            notificationType, toEmail, toName, subject, htmlBody, metadata);

        await _notificationRepository.AddAsync(notification, cancellationToken);

        try
        {
            var message = CreateMimeMessage(toEmail, toName, subject, htmlBody);
            await SendViaSmtpAsync(message, cancellationToken);

            notification.MarkAsSent();
            await _notificationRepository.UpdateAsync(notification, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send tracked email to {Email}", toEmail);
            notification.MarkAsFailed(ex.Message);
            await _notificationRepository.UpdateAsync(notification, cancellationToken);
            return false;
        }
    }

    private MimeMessage CreateMimeMessage(string toEmail, string toName, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = bodyBuilder.ToMessageBody();

        return message;
    }

    private async Task SendViaSmtpAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();
        await client.ConnectAsync(
            _settings.SmtpServer,
            _settings.SmtpPort,
            _settings.SmtpUseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
            cancellationToken);

        await client.AuthenticateAsync(
            _settings.SmtpUsername,
            _settings.SmtpPassword,
            cancellationToken);

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
