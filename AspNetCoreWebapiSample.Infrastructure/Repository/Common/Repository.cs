using AspNetCoreWebapiSample.Domain.Interfaces.Repository.Common;
using AspNetCoreWebapiSample.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AspNetCoreWebapiSample.Infrastructure.Repository.Common
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        
        protected readonly HeroContext _context;

        public Repository(HeroContext context)
        {
            _context = context;
        }
        
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        
        public virtual async Task<bool> IsItExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> InsertAsync(T obj)
        {            
            await _context.Set<T>().AddAsync(obj);
            return obj;
        }

        public virtual void Update(T obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
        }

        public void Update(T obj, params Expression<Func<T, object>>[] properties)
        {
            _context.Set<T>().Attach(obj);
            foreach (var p in properties)
            {
                _context.Entry(obj).Property(p).IsModified = true;
            }
        }
        
        public virtual void Delete(T obj)
        {            
            _context.Set<T>().Remove(obj);
        }

        public void Delete(Expression<Func<T, bool>> predicate)
        {
            var entities = _context.Set<T>().Where(predicate);
            foreach (var entity in entities)
            {
                _context.Entry<T>(entity).State = EntityState.Deleted;
            }
        }

        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
