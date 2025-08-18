using System.Linq.Expressions;

namespace BankingAPI.Application.Interfaces;

public interface IGenericRepositiry<T> where T : class
{
   Task<T?> GetByAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}