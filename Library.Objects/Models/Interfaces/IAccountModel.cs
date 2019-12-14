using System.Threading.Tasks;
using Library.Objects.Helpers.Response;
using Library.Objects.Proxies;

namespace Library.Objects.Models.Interfaces
{
    public interface IAccountModel
    {
        Task<AccessTokenResponse> LogIn(CredentialProxy credentials);
    }
}
