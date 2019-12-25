using System.Collections.Generic;
using System.Threading.Tasks;
using Library.Objects.Context;
using Library.Objects.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace Library.Objects.Repositories.Base
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly DbSet<T> _dbSet;

        protected LibraryContext Context { get; }

        public BaseRepository(LibraryContext context)
        {
            _dbSet = context.Set<T>();
            Context = context;
        }

        public void AddWithoutSave(T entity)
        {
            _dbSet.Add(entity);
        }

        public void AddWithoutSave(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<int> AddAsync(T entity)
        {
            _dbSet.Add(entity);

            await SaveChangesAsync();

            return entity.Id;
        }

        public async Task RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);

            await SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }
    }
}
