using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Library.Objects.Services.Interfaces
{
    public interface IAccessTokenUtility
    {
        string CreateAccessToken(DateTime expires, IEnumerable<Claim> claims);
    }
}
