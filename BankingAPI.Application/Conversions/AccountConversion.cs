using BankingAPI.Application.DTOs;
using BankingAPI.Domain.Entities;

namespace BankingAPI.Application.Conversions;

public class AccountConversion
{
    public static Account ToEntity(AccountDTO accountDTO) => new()
    {
        Id = accountDTO.Id,
        AccountNumber = accountDTO.AccountNumber,
        OwnerName = accountDTO.OwnerName,
        Balance = accountDTO.Balance,
        CreatedAt = accountDTO.CreatedAt,
        IsActive = accountDTO.IsActive
    };

    public static (AccountDTO?, IEnumerable<AccountDTO>?) FromEntity(Account account, IEnumerable<Account>? accounts)
    {
        // Return a single AccountDTO if a single account is passed (account is not null), and accounts is null.
        if (account is not null || accounts is null)
        {
            var _singleAccount = new AccountDTO(account!.Id, account.AccountNumber!, account.OwnerName!, account.Balance!,
                account.CreatedAt!, account.IsActive!);
            return (_singleAccount, null!);
        }
        
        // Return a list of AccountDTOs if accounts is not null.
        if (account is null || accounts is not null)
        {
            var _accounts = accounts.Select(account =>
                new AccountDTO(account!.Id, account.AccountNumber!, account.OwnerName!, account.Balance!,
                    account.CreatedAt!, account.IsActive!)).ToList();
            
            return (null, _accounts);
        }
        
        // Default case if no valid account or account list is provided (shouldn't occur).
        return (null, null);
    }
}