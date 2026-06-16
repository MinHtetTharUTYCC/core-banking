using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.ValueObjects;

namespace CoreBanking.Infrastructure.Persistence;

public class BankingDbContext : DbContext, IApplicationDbContext
{
    public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options) { }
    
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    
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
