using System.Linq.Expressions;
using BankingAPI.Application.Interfaces;
using BankingAPI.Application.Interfaces.Repositories;
using BankingAPI.Application.Logs;
using BankingAPI.Domain.Entities;
using BankingAPI.Domain.Enums;
using BankingAPI.Domain.Responses;

namespace BankingAPI.Application.Services;

public class TransactionServices(ITransactionRepository transactionRepository, IAccountRepository accountRepository)
    : ITransactionService
{
    public async Task<Response> CreateAsync(Transaction transaction)
    {
        try
        {
            // Check SourceAccount
            if (transaction.SourceAccountId.HasValue)
            {
                var source = await accountRepository.GetByAsync(a => a.Id == transaction.SourceAccountId.Value);
                if (source is null)
                    return new Response(false, $"Source account {transaction.SourceAccountId} not found");

                if (transaction.TransactionType == TransactionType.Outcome && source.Balance < transaction.Amount)
                    return new Response(false, $"Insufficient funds in account {source.AccountNumber}");

                // Update balance
                source.Balance += transaction.TransactionType == TransactionType.Income
                    ? transaction.Amount
                    : -transaction.Amount;

                await accountRepository.UpdateAsync(source);
            }

            // Check DestinationAccount for Income-transaction
            if (transaction.DestinationAccountId.HasValue && transaction.TransactionType == TransactionType.Income)
            {
                var dest = await accountRepository.GetByAsync(a => a.Id == transaction.DestinationAccountId.Value);
                if (dest is null)
                    return new Response(false, $"Destination account {transaction.DestinationAccountId} not found");

                dest.Balance += transaction.Amount;
                await accountRepository.UpdateAsync(dest);
            }

            var created = await transactionRepository.CreateAsync(transaction);
            return new Response(true, $"Transaction {created.Id} created successfully");
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);
            throw new InvalidOperationException("Error occured while creating transaction", e);
        }
    }

    public async Task<Response> UpdateAsync(Transaction transaction)
    {
        try
        {
            var existing = await transactionRepository.GetByAsync(t => t.Id == transaction.Id);
            if (existing is null)
                return new Response(false, $"Transaction {transaction.Id} does not exist");

            await transactionRepository.UpdateAsync(transaction);
            return new Response(true, $"Transaction {transaction.Id} updated successfully");
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);
            throw new InvalidOperationException("Error occured while updating transaction", e);
        }
    }

    public async Task<Response> DeleteAsync(int transactionId)
    {
        try
        {
            var existing = await transactionRepository.GetByAsync(t => t.Id == transactionId);
            if (existing is null)
                return new Response(false, $"Transaction {transactionId} not found");

            await transactionRepository.DeleteAsync(existing);
            return new Response(true, $"Transaction {transactionId} deleted successfully");
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);
            throw new InvalidOperationException("Error occured while deleting transaction", e);
        }
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync()
    {
        try
        {
            return await transactionRepository.GetAllAsync();
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);
            throw new InvalidOperationException("Error occured while getting all transactions", e);
        }
    }


    public async Task<Transaction?> GetByAsync(Expression<Func<Transaction, bool>> predicate)
    {
        try
        {
            return await transactionRepository.GetByAsync(predicate);
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);
            throw new InvalidOperationException("Error occured while getting transactions by predicate", e);
        }
    }
}