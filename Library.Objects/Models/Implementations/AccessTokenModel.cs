using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Helpers.Constants;
using Library.Objects.Models.Interfaces;
using Library.Objects.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Library.Objects.Models.Implementations
{
    public class AccessTokenModel : IAccessTokenModel
    {
        private const string VALIDITY_KEY = "AccessTokenValidityInMinutes";

        private readonly IAccessTokenUtility _utility;
        private readonly IConfiguration _configuration;

        public AccessTokenModel(IAccessTokenUtility utility, IConfiguration configuration)
        {
            _utility = utility;
            _configuration = configuration;
        }

        public async Task<string> CreateAsync(User user)
        {
            // validate

            var validity = _configuration.GetValue<double>(VALIDITY_KEY);

            var expires = DateTime.UtcNow.AddMinutes(validity);

            var claim = new Claim(IdentityClaims.EMAIL, user.Email);

            var token = _utility.CreateAccessToken(expires, new[] { claim });

            // SAVE TO DB

            return token;
        }
    }
}
