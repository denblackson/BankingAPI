using System.Linq.Expressions;
using BankingAPI.Application.Interfaces;
using BankingAPI.Application.Interfaces.Repositories;
using BankingAPI.Application.Logs;
using BankingAPI.Domain.Entities;
using BankingAPI.Domain.Responses;

namespace BankingAPI.Application.Services;

public class AccountServices(IAccountRepository accountRepository) : IAccountService
{
    public async Task<Response> CreateAsync(Account entity)
    {
        try
        {
            var existing = await accountRepository.GetByAsync(a => a.AccountNumber == entity.AccountNumber);
            if (existing is not null)
                return new Response(false, $"Account {entity.AccountNumber} already exists");

            var created = await accountRepository.CreateAsync(entity);

            return created.Id > 0
                ? new Response(true, $"Account {entity.AccountNumber} has been added successfully")
                : new Response(false, $"Failed to add account {entity.AccountNumber}");
        }
        catch (Exception e)
        {
            return new Response(false, $"Error while adding account: {e.Message}");
        }
    }

    public async Task<Response> UpdateAsync(Account entity)
    {
        try
        {
            var account = await accountRepository.GetByAsync(a => a.Id == entity.Id);
            if (account is null)
                return new Response(false, $"Account {entity.AccountNumber} does not exist");

            await accountRepository.UpdateAsync(entity);
            return new Response(true, $"Account {entity.AccountNumber} updated successfully");
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);
            throw new InvalidOperationException($"Error occured while updating account {entity.Id}");
        }
    }

    public async Task<Response> DeleteAsync(int id)
    {
        try
        {
            var account = await accountRepository.GetByAsync(a => a.Id == id);
            if (account is null)
                return new Response(false, $"Account {id} does not exist");

            await accountRepository.DeleteAsync(account);
            return new Response(true, $"Account {id} deleted successfully");
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);
            throw new InvalidOperationException($"Error occured while deleting account id {id}");
        }
    }

    public async Task<IEnumerable<Account>> GetAllAsync()
    {
        try
        {
            return await accountRepository.GetAllAsync();
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);
            throw new InvalidOperationException($"Error occured while getting all accounts");
        }
    }

    public async Task<Account?> GetByAsync(Expression<Func<Account, bool>> predicate)
    {
        try
        {
            return await accountRepository.GetByAsync(predicate);
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);
            throw new InvalidOperationException($"Error occured while getting account by predicate {predicate}");
        }
    }
}