using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.ValueObjects;

namespace CoreBanking.Infrastructure.Persistence;

public class BankingDbContext : DbContext, IApplicationDbContext
{
    private readonly IDomainEventDispatcher _dispatcher;
    
    public BankingDbContext(DbContextOptions<BankingDbContext> options, IDomainEventDispatcher dispatcher) 
        : base(options) 
    { 
        _dispatcher = dispatcher;
    }
    
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker.Entries<BaseEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var entity in entities)
            entity.ClearDomainEvents();

        foreach (var domainEvent in domainEvents)
            await _dispatcher.DispatchAsync(domainEvent, cancellationToken);

        return result;
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
        
        builder.Ignore(a => a.Transactions);
        builder.Ignore(a => a.DomainEvents);
        
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Amount)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(t => t.BalanceBefore)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(t => t.BalanceAfter)
            .HasPrecision(18, 2)
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
