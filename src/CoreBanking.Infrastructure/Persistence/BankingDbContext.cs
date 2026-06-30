using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.ValueObjects;

namespace CoreBanking.Infrastructure.Persistence;

public class BankingDbContext(DbContextOptions<BankingDbContext> options, IDomainEventDispatcher dispatcher)
    : DbContext(options), IApplicationDbContext
{
    private int _saveChangesDepth;
    
    public DbSet<Account> Accounts => Set<Account>();
    
    public DbSet<Transaction> Transactions => Set<Transaction>();
    
    public DbSet<Loan> Loans => Set<Loan>();
    
    public DbSet<Notification> Notifications => Set<Notification>();
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_saveChangesDepth > 0)
            return await base.SaveChangesAsync(cancellationToken);
            
        _saveChangesDepth++;
        try
        {
            // 1. Snag the entities and their events before writing to database
            var entities = ChangeTracker.Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Count > 0)
                .ToList();

            var domainEvents = entities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            // 2. Clear events from the instances so they don't fire twice
            foreach (var entity in entities)
                entity.ClearDomainEvents();

            // 3. Commit EVERYTHING tracked up to this point in a single pass
            var result = await base.SaveChangesAsync(cancellationToken);

            // 4. Run your domain event dispatching down the line
            foreach (var domainEvent in domainEvents)
                await dispatcher.DispatchAsync(domainEvent, cancellationToken);

            return result;
        }
        finally
        {
            _saveChangesDepth--;
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankingDbContext).Assembly);
    }
}

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.AccountNumber)
            .HasConversion(
                v => v.Value,
                v => new AccountNumber(v))
            .HasMaxLength(12)
            .IsRequired();
        
        builder.Property(a => a.OwnerName)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(a => a.OwnerEmail)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.OwnsOne(a => a.Balance, b =>
        {
            b.Property(m => m.Amount).HasColumnName("BalanceAmount");
            b.Property(m => m.Currency).HasColumnName("BalanceCurrency");
        });

        builder.Ignore(a => a.Transactions);
        builder.Ignore(a => a.DomainEvents);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(n => n.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(n => n.RecipientEmail)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(n => n.RecipientName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(n => n.Subject)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(n => n.Body)
            .IsRequired();

        builder.Property(n => n.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(n => n.Metadata)
            .HasMaxLength(4000);

        builder.Ignore(n => n.DomainEvents);

        builder.HasQueryFilter(n => !n.IsDeleted);

        builder.HasIndex(n => n.RecipientEmail);
        builder.HasIndex(n => n.Type);
        builder.HasIndex(n => n.Status);
    }
}

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Amount)
            .IsRequired();
        
        builder.Property(t => t.BalanceBefore)
            .IsRequired();
        
        builder.Property(t => t.BalanceAfter)
            .IsRequired();
        
        builder.Property(t => t.Description)
            .HasMaxLength(500);
        
        builder.Property(t => t.ReferenceNumber)
            .HasMaxLength(50);
        
        builder.HasOne(t => t.Account)
            .WithMany()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(t => t.RelatedAccount)
            .WithMany()
            .HasForeignKey(t => t.RelatedAccountId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.LoanNumber)
            .HasConversion(
                v => v.Value,
                v => new LoanNumber(v))
            .HasMaxLength(12)
            .IsRequired();

        builder.Property(l => l.InterestRate)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(l => l.TermMonths)
            .IsRequired();

        builder.Property(l => l.MonthlyPayment)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.OwnsOne(l => l.PrincipalAmount, b =>
        {
            b.Property(m => m.Amount).HasColumnName("PrincipalAmount");
            b.Property(m => m.Currency).HasColumnName("PrincipalCurrency");
        });

        builder.OwnsOne(l => l.OutstandingBalance, b =>
        {
            b.Property(m => m.Amount).HasColumnName("OutstandingAmount");
            b.Property(m => m.Currency).HasColumnName("OutstandingCurrency");
        });

        builder.Property(l => l.RejectionReason)
            .HasMaxLength(500);

        builder.HasOne(l => l.Account)
            .WithMany()
            .HasForeignKey(l => l.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(l => l.DomainEvents);

        builder.HasQueryFilter(l => !l.IsDeleted);
    }
}
