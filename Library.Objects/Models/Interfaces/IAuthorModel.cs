using System.Threading.Tasks;
using Library.Objects.Helpers.Request;
using Library.Objects.Helpers.Response;
using Library.Objects.Proxies;

namespace Library.Objects.Models.Interfaces
{
    public interface IAuthorModel
    {
        Task<IdResponse> CreateAuthorAsync(AuthorProxy author);
        Task UpdateAuthorAsync(int id, AuthorProxy author);
        Task<AuthorProxy> GetAuthorByIdAsync(int id);
        Task DeleteAuthorAsync(int id);
        Task<PageResponse<AuthorProxy>> GetAuthorPageAsync(PageRequest pageRequest);
        Task ValidateExistingAuthorAsync(int id);
    }
}
