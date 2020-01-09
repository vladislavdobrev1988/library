using System.Threading.Tasks;
using Library.Objects.Context;
using Library.Objects.Entities;
using Library.Objects.Helpers.Extensions;
using Library.Objects.Helpers.Request;
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

        public async Task<bool> AuthorHasBooksAsync(int authorId)
        {
            return await Context.Books.AnyAsync(x => x.AuthorId == authorId);
        }

        public async Task<Author[]> GetPageAsync(PageRequest pageRequest)
        {
            var query = Context.Authors.AsNoTracking().GetPage(pageRequest);

            return await query.ToArrayAsync();
        }
    }
}
