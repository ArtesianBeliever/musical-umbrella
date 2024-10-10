using System.Collections.Generic;
using System.Linq.Expressions;

namespace WebApplication2.Models;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id, QueryOptions<T> options);
    
    Task<IEnumerable<T>> GetAllByIdAsync<TKey>(TKey id, string propertyName, QueryOptions<T> options);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}