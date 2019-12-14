using Library.Objects.Entities;
using Library.Objects.Proxies;
using System.Threading.Tasks;

namespace Library.Objects.Models.Interfaces
{
    public interface IUserModel
    {
        Task CreateUserAsync(UserProxy user);
        Task<User> GetByEmailAsync(string email);
    }
}
