using BankingAPI.Application.Interfaces;
using BankingAPI.Application.Interfaces.Repositories;
using BankingAPI.Application.Services;
using BankingAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace BankingAPI.Infrastructure.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddBankingServices<TContext>(this IServiceCollection services,
        IConfiguration config, string filename)
        where TContext : DbContext
    {
        // Database context (SQL Server with retry policy)
        services.AddDbContext<TContext>(options =>
            options.UseSqlServer(
                config.GetConnectionString("BankingConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure()
            ));

        // Configure Serilog for logging
        Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Debug()
            .WriteTo.Console().WriteTo.File(path: $"{filename}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day).CreateLogger();


        // Register repositories
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        
        // Register services
        services.AddScoped<ITransactionService, TransactionServices>();
        services.AddScoped<IAccountService, AccountServices>();

        return services;
    }
}