using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.Enums;
using CoreBanking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoreBanking.Infrastructure.Services;

public class NotificationRepository(BankingDbContext context) : INotificationRepository
{
    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetByRecipientEmailAsync(
        string email, CancellationToken cancellationToken = default)
    {
        return await context.Notifications
            .Where(n => n.RecipientEmail == email && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetByTypeAsync(
        NotificationType type, CancellationToken cancellationToken = default)
    {
        return await context.Notifications
            .Where(n => n.Type == type && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await context.Notifications.AddAsync(notification, cancellationToken);
    }
    
    public Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        context.Notifications.Update(notification);
        return Task.CompletedTask;
    }
}
