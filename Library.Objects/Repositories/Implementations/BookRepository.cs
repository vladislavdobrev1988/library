using System.Threading.Tasks;
using Library.Objects.Context;
using Library.Objects.Entities;
using Library.Objects.Repositories.Base;
using Library.Objects.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Objects.Repositories.Implementations
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        public BookRepository(LibraryContext context) : base(context) { }

        public async Task<Book> GetByTitleAsync(string title)
        {
            return await Context.Books.FirstOrDefaultAsync(x => x.Title == title);
        }
    }
}
