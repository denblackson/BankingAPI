// using System.Linq.Expressions;
// using System.Runtime.InteropServices;
// using BankingAPI.Application.Interfaces;
// using BankingAPI.Application.Logs;
// using BankingAPI.Domain.Entities;
// using BankingAPI.Domain.Responses;
// using BankingAPI.Infrastructure.Data;
// using Microsoft.EntityFrameworkCore;

// namespace BankingAPI.Infrastructure.Repositories;

// public class AccountRepository(BankingApiDbContext context) : IAccount
// {
//     public async Task<Response> CreateAsync(Account entity)
//     {
//         try
//         {
//             var getAccount = await GetByAsync(_ => _.AccountNumber.Equals(entity.AccountNumber));
//             if (getAccount is not null && !string.IsNullOrEmpty(getAccount.AccountNumber))
//                 return new Response(false, $"Account {entity.AccountNumber} already exists");
//
//             var currentEntity = context.Accounts.Add(entity).Entity;
//             await context.SaveChangesAsync();
//
//             if (currentEntity is not null && currentEntity.Id > 0)
//                 return new Response(true, $"Account {entity.AccountNumber} has been added to db successfully");
//             else
//                 return new Response(false, $"Error occured while adding account {entity.AccountNumber}");
//         }
//         catch (Exception e)
//         {
//             LogException.LogExceptions(e);
//             return new Response(false, $"Error occured while adding account {entity.AccountNumber}");
//         }
//     }
//
//     public async Task<Response> UpdateAsync(Account entity)
//     {
//         try
//         {
//             var account = await FindByIdAsync(entity.Id);
//             if (account is null)
//                 return new Response(false, $"Account {entity.AccountNumber} does not exist");
//
//             context.Entry(account).State = EntityState.Detached; // to avoid possible conflicts
//             context.Accounts.Update(entity);
//             await context.SaveChangesAsync();
//
//             return new Response(true, $"Account {entity.AccountNumber} is updated successfully");
//         }
//         catch (Exception e)
//         {
//             LogException.LogExceptions(e);
//             return new Response(false, $"Error occured while updating account {entity.AccountNumber}");
//         }
//     }
//
//     public async Task<Response> DeleteAsync(Account entity)
//     {
//         try
//         {
//             var account = await FindByIdAsync(entity.Id);
//
//             if (account is null)
//             {
//                 return new Response(false, $"Account {entity.AccountNumber} does not exist");
//             }
//
//             context.Accounts.Remove(account);
//             await context.SaveChangesAsync();
//
//             return new Response(true, $"Account {entity.AccountNumber} is deleted successfully");
//         }
//         catch (Exception e)
//         {
//             LogException.LogExceptions(e);
//             return new Response(false, $"Error occured while deleting Account {entity.AccountNumber}");
//         }
//     }
//
//     public async Task<IEnumerable<Account>> GetAllAsync()
//     {
//         try
//         {
//             var accounts = await context.Accounts.AsNoTracking().ToListAsync();
//             return accounts is not null ? accounts : null!;
//         }
//         catch (Exception e)
//         {
//             LogException.LogExceptions(e);
//             throw new InvalidOperationException("Error occured while getting all accounts", e);
//         }
//     }
//
//     public async Task<Account> FindByIdAsync(int id)
//     {
//         try
//         {
//             var account = await context.Accounts.FindAsync(id);
//             return account is not null ? account : null;
//         }
//         catch (Exception e)
//         {
//             LogException.LogExceptions(e);
//             throw new InvalidOleVariantTypeException($"Error occured while getting account {id}", e);
//         }
//     }
//
//     public async Task<Account> GetByAsync(Expression<Func<Account, bool>> predicate)
//     {
//         try
//         {
//             var account =
//                 await context.Accounts.Where(predicate).FirstOrDefaultAsync()!; // allows to pass a filtering condition;
//             return account is not null ? account : null!;
//         }
//         catch (Exception e)
//         {
//             LogException.LogExceptions(e);
//             throw new InvalidOleVariantTypeException($"Error occured while GettingBY account {predicate}", e);
//         }
//     }
// }

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