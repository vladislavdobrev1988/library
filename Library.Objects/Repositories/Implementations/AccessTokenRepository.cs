using System.Threading.Tasks;
using Library.Objects.Context;
using Library.Objects.Entities;
using Library.Objects.Repositories.Base;
using Library.Objects.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Objects.Repositories.Implementations
{
    public class AccessTokenRepository : BaseRepository<AccessToken>, IAccessTokenRepository
    {
        public AccessTokenRepository(LibraryContext context) : base(context) { }

        public async Task<AccessToken> GetByToken(string token)
        {
            return await DbSet.FirstOrDefaultAsync(x => x.Token == token);
        }
    }
}
