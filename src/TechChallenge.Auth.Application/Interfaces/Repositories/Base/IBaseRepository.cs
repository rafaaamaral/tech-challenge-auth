using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace TechChallenge.Auth.Application.Interfaces.Repositories.Base
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByUniqueCodeAsync(Guid uniqueCode);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task RemoveAsync(T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync();
    }
}
