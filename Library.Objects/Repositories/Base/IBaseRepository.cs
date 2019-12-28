using Library.Objects.Entities.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Objects.Repositories.Base
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        void AddWithoutSave(T entity);
        void AddWithoutSave(IEnumerable<T> entities);
        Task<int> AddAsync(T entity);
        Task<T> GetByIdAsync(int id);
        Task RemoveAsync(T entity);
        Task<int> GetCountAsync();
        Task<int> SaveChangesAsync();
    }
}
