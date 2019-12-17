using System.Collections.Generic;
using System.Security.Claims;

namespace Library.Objects.Services.Interfaces
{
    public interface IAccessTokenUtility
    {
        string CreateAccessToken(IEnumerable<Claim> claims);
        ClaimsIdentity GetIdentity(string token);
    }
}
