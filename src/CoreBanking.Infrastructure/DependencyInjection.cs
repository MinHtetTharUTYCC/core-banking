using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Infrastructure.Persistence;
using CoreBanking.Infrastructure.Repositories;

namespace CoreBanking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BankingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<BankingDbContext>());
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        
        return services;
    }
}
