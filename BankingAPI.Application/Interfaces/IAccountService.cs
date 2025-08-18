using System.Linq.Expressions;
using BankingAPI.Domain.Entities;
using BankingAPI.Domain.Responses;

namespace BankingAPI.Application.Interfaces;

public interface IAccountService
{
    Task<Response> CreateAsync(Account entity);
    Task<Response> UpdateAsync(Account entity);
    Task<Response> DeleteAsync(int id);
    Task<IEnumerable<Account>> GetAllAsync();
    Task<Account?> GetByAsync(Expression<Func<Account, bool>> predicate);
}