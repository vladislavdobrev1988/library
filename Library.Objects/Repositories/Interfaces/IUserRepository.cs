using Library.Objects.Entities;
using Library.Objects.Repositories.Base;
using System.Threading.Tasks;

namespace Library.Objects.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
    }
}
