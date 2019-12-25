using System.Threading.Tasks;
using Library.Objects.Context;
using Library.Objects.Entities;
using Library.Objects.Repositories.Base;
using Library.Objects.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Objects.Repositories.Implementations
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(LibraryContext context) : base(context) { }

        public async Task<User> GetByEmail(string email)
        {
            return await Context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
