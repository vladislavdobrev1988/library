using Library.Objects.Entities.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Objects.Repositories.Base
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        void Add(T entity);
        void Add(IEnumerable<T> entities);
        Task<T> GetByIdAsync(int id);
        Task<int> SaveChangesAsync();
    }
}
