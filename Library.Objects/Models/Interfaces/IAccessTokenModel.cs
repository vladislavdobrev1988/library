using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Library.Objects.Entities;

namespace Library.Objects.Models.Interfaces
{
    public interface IAccessTokenModel
    {
        string CreateAccessToken(User user);
        ClaimsIdentity GetIdentity(string token);
    }
}
