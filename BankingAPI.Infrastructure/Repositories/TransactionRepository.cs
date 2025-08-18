// using System.Linq.Expressions;
// using System.Runtime.InteropServices;
// using BankingAPI.Application.Interfaces;
// using BankingAPI.Application.Logs;
// using BankingAPI.Domain.Entities;
// using BankingAPI.Domain.Responses;
// using BankingAPI.Infrastructure.Data;
// using Microsoft.EntityFrameworkCore;
//
// namespace BankingAPI.Infrastructure.Repositories;
//
// public class TransactionRepository(BankingApiDbContext context) : ITransactionRepository
// {
//     public async Task<Response> CreateAsync(Transaction entity)
//     {
//         try
//         {
//             var getTransaction = await GetByAsync(_ => _.Id.Equals(entity.Id));
//             if (getTransaction is not null && getTransaction.Id > 0)
//                 return new Response(false, $"Transaction {entity.Id} already exists");
//
//             var currentEntity = context.Transactions.Add(entity).Entity;
//             await context.SaveChangesAsync();
//
//             if (currentEntity is not null && currentEntity.Id > 0)
//                 return new Response(true, $"Transaction {entity.Id} has been added to db successfully");
//             else
//                 return new Response(false, $"Error occured while adding Transaction {entity.Id}");
//         }
//         catch (Exception e)
//         {
//             LogException.LogExceptions(e);
//             return new Response(false, $"Error occured while adding Transaction {entity.Id}");
//         }
//     }
//
//     public async Task<Response> UpdateAsync(Transaction entity)
//     {
//         try
//         {
//             var transaction = await FindByIdAsync(entity.Id);
//             if (transaction is null)
//                 return new Response(false, $"Transaction {entity.Id} does not exist");
//
//             context.Entry(transaction).State = EntityState.Detached;
//             context.Transactions.Update(entity);
//             await context.SaveChangesAsync();
//
//             return new Response(true, $"Transaction {entity.Id} is updated successfully");
//         }
//         catch (Exception e)
//         {
//             LogException.LogExceptions(e);
//             return new Response(false, $"Error occured while updating Transaction {entity.Id}");
//         }
//     }
//
//     public async Task<Response> DeleteAsync(Transaction entity)
//     {
//         try
//         {
//             var transaction = await FindByIdAsync(entity.Id);
//
//             if (transaction is null)
//             {
//                 return new Response(false, $"Transaction {entity.Id} does not exist");
//             }
//
//             context.Transactions.Remove(transaction);
//             await context.SaveChangesAsync();
//
//             return new Response(true, $"Transaction {entity.Id} is deleted successfully");
//         }
//         catch (Exception e)
//         {
//             LogException.LogExceptions(e);
//             return new Response(false, $"Error occured while deleting Transaction {entity.Id}");
//         }
//     }
//
//     public async Task<IEnumerable<Transaction>> GetAllAsync()
//     {
//         try
//         {
//             var transactions = await context.Transactions.AsNoTracking().ToListAsync();
//             return transactions is not null ? transactions : null!;
//         }
//         catch (Exception e)
//         {
//             LogException.LogExceptions(e);
//             throw new InvalidOperationException("Error occured while getting all transactions", e);
//         }
//     }
//
//     public async Task<Transaction> FindByIdAsync(int id)
//     {
//         try
//         {
//             var transaction = await context.Transactions.FindAsync(id);
//             return transaction is not null ? transaction : null;
//         }
//         catch (Exception e)
//         {
//             LogException.LogExceptions(e);
//             throw new InvalidOleVariantTypeException($"Error occured while getting Transaction {id}", e);
//         }
//     }
//
//     public async Task<Transaction> GetByAsync(Expression<Func<Transaction, bool>> predicate)
//     {
//         try
//         {
//             var transaction =
//                 await context.Transactions.Where(predicate).FirstOrDefaultAsync()!;
//             return transaction is not null ? transaction : null!;
//         }
//         catch (Exception e)
//         {
//             LogException.LogExceptions(e);
//             throw new InvalidOleVariantTypeException($"Error occured while GettingBY Transaction {predicate}", e);
//         }
//     }
// }


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