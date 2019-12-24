using System.Threading.Tasks;
using Library.Objects.Proxies;

namespace Library.Objects.Models.Interfaces
{
    public interface IAuthorModel
    {
        Task<int> CreateAuthorAsync(AuthorProxy author);
        Task<AuthorProxy> GetAuthorByIdAsync(int id);
    }
}
