using Library.Objects.Context;
using Library.Objects.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Objects.Repositories.Base
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly LibraryContext _context;

        protected DbSet<T> DbSet { get; }

        public BaseRepository(LibraryContext context)
        {
            _context = context;
            DbSet = _context.Set<T>();
        }

        public void AddWithoutSave(T entity)
        {
            DbSet.Add(entity);
        }

        public void AddWithoutSave(IEnumerable<T> entities)
        {
            DbSet.AddRange(entities);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public async Task<int> AddAsync(T entity)
        {
            DbSet.Add(entity);

            await SaveChangesAsync();

            return entity.Id;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
