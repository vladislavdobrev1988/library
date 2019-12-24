using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Repositories.Base;

namespace Library.Objects.Repositories.Interfaces
{
    public interface IAuthorRepository : IBaseRepository<Author>
    {
        Task<Author> GetByNameAsync(string firstName, string lastName);
    }
}
