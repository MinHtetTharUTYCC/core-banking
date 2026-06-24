using CoreBanking.Domain.Entities;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Application.Common.Interfaces;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetByRecipientEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetByTypeAsync(NotificationType type, CancellationToken cancellationToken = default);
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
}
