using CoreBanking.Domain.Common;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Domain.Entities;

public class Notification : BaseEntity
{
    public NotificationType Type { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
    public string? ErrorMessage { get; set; }
    public DateTime? SentAt { get; set; }
    public string? Metadata { get; set; }

    public static Notification Create(
        NotificationType type,
        string recipientEmail,
        string recipientName,
        string subject,
        string body,
        string? metadata = null)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            Type = type,
            RecipientEmail = recipientEmail,
            RecipientName = recipientName,
            Subject = subject,
            Body = body,
            Status = NotificationStatus.Pending,
            Metadata = metadata,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    public void MarkAsSent()
    {
        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string errorMessage)
    {
        Status = NotificationStatus.Failed;
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.UtcNow;
    }
}
