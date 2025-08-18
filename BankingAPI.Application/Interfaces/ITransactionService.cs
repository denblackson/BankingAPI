using System.Linq.Expressions;
using BankingAPI.Domain.Entities;
using BankingAPI.Domain.Responses;

namespace BankingAPI.Application.Interfaces;

public interface ITransactionService
{
    Task<Response> CreateAsync(Transaction transaction);
    Task<Response> UpdateAsync(Transaction transaction);
    Task<Response> DeleteAsync(int transactionId);
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<Transaction?> GetByAsync(Expression<Func<Transaction, bool>> predicate);
}