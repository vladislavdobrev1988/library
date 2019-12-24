using Library.Objects.Context;
using Library.Objects.Entities;
using Library.Objects.Repositories.Base;
using Library.Objects.Repositories.Interfaces;

namespace Library.Objects.Repositories.Implementations
{
    public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(LibraryContext context) : base(context) { }
    }
}
