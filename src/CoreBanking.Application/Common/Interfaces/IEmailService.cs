using CoreBanking.Domain.Enums;

namespace CoreBanking.Application.Common.Interfaces;

public interface IEmailService
{
    Task<bool> SendAsync(string toEmail, string toName, string subject, string htmlBody, CancellationToken cancellationToken = default);

    Task<bool> SendTemplatedAsync<TModel>(
        string toEmail,
        string toName,
        string templateKey,
        TModel model,
        string subject,
        CancellationToken cancellationToken = default);

    Task<bool> SendAndTrackAsync<TModel>(
        string toEmail,
        string toName,
        string templateKey,
        TModel model,
        string subject,
        NotificationType notificationType,
        string? metadata = null,
        CancellationToken cancellationToken = default);
}

public interface IEmailServiceCopy
{
    Task<bool> SendAsync(string toEmail, string toName, string subject, string htmlBody, CancellationToken ct = default);

    Task<bool> SendTemplatedAsync<TModel>(
        string toEmail,
        string toName,
        string templateKey,
        TModel model,
        string subject,
        CancellationToken ct = default);

    Task<bool> SendAndTrackAsync<TModel>(
        string toEmail,
        string toName,
        string templateKey,
        TModel model,
        string subject,
        NotificationType notificationType,
        string? metadata = null,
        CancellationToken ct = default);
}