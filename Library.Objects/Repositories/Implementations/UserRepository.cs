using Library.Objects.Context;
using Library.Objects.Entities;
using Library.Objects.Repositories.Base;
using Library.Objects.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Library.Objects.Repositories.Implementations
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(LibraryContext context) : base(context) { }

        public async Task<User> GetByEmail(string email)
        {
            return await DbSet.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
