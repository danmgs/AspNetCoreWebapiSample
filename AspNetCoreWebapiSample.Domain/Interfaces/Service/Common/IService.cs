using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreWebapiSample.Domain.Interfaces.Service.Common
{
    public interface IService<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> InsertAsync(T obj);
        Task<T> UpdateAsync(T obj);
        Task DeleteAsync(int id);
        Task<bool> IsIdExistsAsync(int id);
    }
}
