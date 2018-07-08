using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AspNetCoreWebapiSample.Domain.Interfaces.Repository.Common
{
    public interface IRepository<T> : IDisposable where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetByIdAsync(int id);
        Task<T> InsertAsync(T obj);
        void Update(T obj);
        void Update(T obj, params Expression<Func<T, object>>[] properties);
        void Delete(T obj);
        void Delete(Expression<Func<T, bool>> predicate);
        Task SaveChangesAsync();        
    }
}
