using System.Threading.Tasks;

namespace Library.Helpers.Authorization
{
    public interface IAccessTokenStore
    {
        Task<bool> IsValidAccessTokenAsync(string accessToken);
    }
}
