using System.Collections.Concurrent;
using System.Reflection;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.Enums;
using CoreBanking.Infrastructure.Configuration;
using Fluid;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CoreBanking.Infrastructure.Services;

public class EmailService(
    IOptions<EmailSettings> settings,
    INotificationRepository notificationRepository,
    ILogger<EmailService> logger) : IEmailService
{
    private readonly EmailSettings _settings = settings.Value;
    private static readonly ConcurrentDictionary<string, IFluidTemplate> TemplateCache = new();

    public async Task<bool> SendAsync(
        string toEmail, string toName, string subject, string htmlBody,
        CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled)
        {
            logger.LogWarning("Email sending is disabled in configuration");
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
            logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendTemplatedAsync<TModel>(
        string toEmail, string toName, string templateKey, TModel model,
        string subject, CancellationToken cancellationToken = default)
    {
        var htmlBody = RenderTemplate(templateKey, model);
        return await SendAsync(toEmail, toName, subject, htmlBody, cancellationToken);
    }

    public async Task<bool> SendAndTrackAsync<TModel>(
        string toEmail, string toName, string templateKey, TModel model,
        string subject, NotificationType notificationType,
        string? metadata = null, CancellationToken cancellationToken = default)
    {
        var htmlBody = RenderTemplate(templateKey, model);

        var notification = Notification.Create(
            notificationType, toEmail, toName, subject, htmlBody, metadata);

        await notificationRepository.AddAsync(notification, cancellationToken);

        try
        {
            var message = CreateMimeMessage(toEmail, toName, subject, htmlBody);
            await SendViaSmtpAsync(message, cancellationToken);

            notification.MarkAsSent();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send tracked email to {Email}", toEmail);
            notification.MarkAsFailed(ex.Message);
            return false;
        }
    }

    private static string RenderTemplate<TModel>(string templateKey, TModel model)
    {
        var template = TemplateCache.GetOrAdd(templateKey, key =>
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{assembly.GetName().Name}.EmailTemplates.{key}.liquid";
            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException($"Email template not found: {resourceName}");
            using var reader = new StreamReader(stream);
            return new FluidParser().Parse(reader.ReadToEnd());
        });

        var context = new TemplateContext(model, true);
        return template.Render(context);
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

        await client.SendAsync(message);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
