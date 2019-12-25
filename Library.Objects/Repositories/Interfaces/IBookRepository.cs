using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Repositories.Base;

namespace Library.Objects.Repositories.Interfaces
{
    public interface IBookRepository : IBaseRepository<Book>
    {
        Task<Book> GetByTitleAsync(string title);
        Task<Book[]> GetByAuthorAsync(int authorId);
    }
}
