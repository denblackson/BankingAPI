using System.Linq.Expressions;
using BankingAPI.Application.Interfaces;
using BankingAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BankingAPI.Domain.Entities;

namespace BankingAPI.Infrastructure.Repositories;

public class TransactionRepository(BankingApiDbContext context) : ITransactionRepository
{
    public async Task<Transaction> CreateAsync(Transaction entity)
    {
        var created = context.Transactions.Add(entity).Entity;
        await context.SaveChangesAsync();
        return created;
    }

    public async Task UpdateAsync(Transaction entity)
    {
        var trackedEntity = context.Transactions.Local.FirstOrDefault(a => a.Id == entity.Id);
        if (trackedEntity != null)
            context.Entry(trackedEntity).State = EntityState.Detached;
        
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Transaction entity)
    {
        context.Transactions.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync() =>
        await context.Transactions.AsNoTracking().ToListAsync();

    public async Task<Transaction?> FindByIdAsync(int id) =>
        await context.Transactions.FindAsync(id);

    public async Task<Transaction?> GetByAsync(Expression<Func<Transaction, bool>> predicate) =>
        await context.Transactions.FirstOrDefaultAsync(predicate);

    public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(int accountId) =>
        await context.Transactions
            .Where(t => t.SourceAccountId == accountId || t.DestinationAccountId == accountId)
            .AsNoTracking()
            .ToListAsync();
}