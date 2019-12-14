using System;
using System.Threading.Tasks;
using Library.Objects.Entities;

namespace Library.Objects.Models.Interfaces
{
    public interface IAccessTokenModel
    {
        Task<string> CreateAccessTokenAsync(User user);
        Task<bool> IsValidAccessTokenAsync(string token);
    }
}
