using System.Threading.Tasks;
using Library.Objects.Context;
using Library.Objects.Entities;
using Library.Objects.Repositories.Base;
using Library.Objects.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Objects.Repositories.Implementations
{
    public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(LibraryContext context) : base(context) { }

        public async Task<Author> GetByNameAsync(string firstName, string lastName)
        {
            return await Context.Authors.FirstOrDefaultAsync(x => x.FirstName == firstName && x.LastName == lastName);
        }

        public async Task<int> GetBookCount(int authorId)
        {
            return await Context.Books.CountAsync(x => x.AuthorId == authorId);
        }
    }
}
