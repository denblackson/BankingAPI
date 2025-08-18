using System.Linq.Expressions;
using BankingAPI.Application.Interfaces.Repositories;
using BankingAPI.Domain.Entities;
using BankingAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingAPI.Infrastructure.Repositories;

public class AccountRepository(BankingApiDbContext context) : IAccountRepository
{
    public async Task<Account?> GetByAsync(Expression<Func<Account, bool>> predicate) =>
        await context.Accounts.FirstOrDefaultAsync(predicate);

    public async Task<IEnumerable<Account>> GetAllAsync() =>
        await context.Accounts.AsNoTracking().ToListAsync();

    public async Task<Account> CreateAsync(Account entity)
    {
        var created = context.Accounts.Add(entity).Entity;
        await context.SaveChangesAsync();
        return created;
    }

    public async Task UpdateAsync(Account entity)
    {
        var trackedEntity = context.Accounts.Local.FirstOrDefault(a => a.Id == entity.Id);
        if (trackedEntity != null)
            context.Entry(trackedEntity).State = EntityState.Detached;

        context.Accounts.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Account entity)
    {
        context.Accounts.Remove(entity);
        await context.SaveChangesAsync();
    }
}