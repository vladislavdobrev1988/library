using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Repositories.Base;

namespace Library.Objects.Repositories.Interfaces
{
    public interface IAccessTokenRepository : IBaseRepository<AccessToken>
    {
        Task<AccessToken> GetByToken(string token);
    }
}
