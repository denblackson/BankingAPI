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
    public async Task<Response> DepositAsync(string accountNumber, decimal amount, string description = "")
    {
        try
        {
            var account = await accountRepository.GetByAsync(a => a.AccountNumber == accountNumber);
            if (account == null) return new Response(false, $"Account {accountNumber} not found");

            account.Balance += amount;
            await accountRepository.UpdateAsync(account);

            var transaction = new Transaction
            {
                TransactionType = TransactionType.Income,
                Amount = amount,
                DestinationAccount = account,
                Description = description
            };

            var created = await transactionRepository.CreateAsync(transaction);
            return new Response(true, $"Deposit {amount} to {accountNumber} successful, transaction {created.Id}");
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);
            throw new InvalidOperationException("Error occured while conducting depositing");
        }
    }

    public async Task<Response> WithdrawAsync(string accountNumber, decimal amount, string description = "")
    {
        try
        {
            var account = await accountRepository.GetByAsync(a => a.AccountNumber == accountNumber);
            if (account == null) return new Response(false, $"Account {accountNumber} not found");

            if (account.Balance < amount)
                return new Response(false, $"Insufficient funds in account {accountNumber}");

            account.Balance -= amount;
            await accountRepository.UpdateAsync(account);

            var transaction = new Transaction
            {
                TransactionType = TransactionType.Outcome,
                Amount = amount,
                SourceAccount = account,
                Description = description
            };

            var created = await transactionRepository.CreateAsync(transaction);
            return new Response(true, $"Withdraw {amount} from {accountNumber} successful, transaction {created.Id}");
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);
            throw new InvalidOperationException("Error occured while conducting Withdrawing");
        }
    }

    public async Task<Response> TransferAsync(string sourceAccountNumber, string destinationAccountNumber,
        decimal amount, string description = "")
    {
        try
        {
            var source = await accountRepository.GetByAsync(a => a.AccountNumber == sourceAccountNumber);
            var destination = await accountRepository.GetByAsync(a => a.AccountNumber == destinationAccountNumber);

            if (source == null) return new Response(false, $"Source account {sourceAccountNumber} not found");
            if (destination == null)
                return new Response(false, $"Destination account {destinationAccountNumber} not found");
            if (source.Balance < amount)
                return new Response(false, $"Insufficient funds in account {sourceAccountNumber}");

            source.Balance -= amount;
            destination.Balance += amount;

            await accountRepository.UpdateAsync(source);
            await accountRepository.UpdateAsync(destination);

            var transaction = new Transaction
            {
                TransactionType = TransactionType.Outcome,
                Amount = amount,
                SourceAccount = source,
                DestinationAccount = destination,
                Description = description
            };

            var created = await transactionRepository.CreateAsync(transaction);
            return new Response(true,
                $"Transfer {amount} from {sourceAccountNumber} to {destinationAccountNumber} successful, transaction {created.Id}");
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);
            throw new InvalidOperationException("Error occured while conducting transfering");
        }
    }


    public async Task<Response> DeleteAsync(int transactionId)
    {
        try
        {
            var existing = await transactionRepository.GetByAsync(t => t.Id == transactionId);
            if (existing is null)
                return new Response(false, $"Transaction {transactionId} not found");

            // Rollback balance when deleting a transaction
            if (existing.SourceAccountId.HasValue)
            {
                var source = await accountRepository.GetByAsync(a => a.Id == existing.SourceAccountId.Value);
                if (source != null)
                {
                    source.Balance += existing.Amount; // return funds
                    await accountRepository.UpdateAsync(source);
                }
            }

            if (existing.DestinationAccountId.HasValue)
            {
                var dest = await accountRepository.GetByAsync(a => a.Id == existing.DestinationAccountId.Value);
                if (dest != null)
                {
                    dest.Balance -= existing.Amount; // deduct funds
                    await accountRepository.UpdateAsync(dest);
                }
            }

            await transactionRepository.DeleteAsync(existing);
            return new Response(true, $"Transaction {transactionId} deleted successfully");
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);
            throw new InvalidOperationException("Error occured while deleting transaction");
        }
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync()
    {
        return await transactionRepository.GetAllAsync();
    }


    public async Task<Transaction?> GetByAsync(Expression<Func<Transaction, bool>> predicate)
    {
        return await transactionRepository.GetByAsync(predicate);
    }


    // public async Task<Response> UpdateAsync(Transaction transaction)
    // {
    //     var existing = await transactionRepository.GetByAsync(t => t.Id == transaction.Id);
    //     if (existing is null)
    //         return new Response(false, $"Transaction {transaction.Id} does not exist");
    //
    //     await transactionRepository.UpdateAsync(transaction);
    //     return new Response(true, $"Transaction {transaction.Id} updated successfully");
    // }
}