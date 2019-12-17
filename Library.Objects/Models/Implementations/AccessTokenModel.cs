using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Helpers.Constants;
using Library.Objects.Models.Interfaces;
using Library.Objects.Repositories.Interfaces;
using Library.Objects.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Library.Objects.Models.Implementations
{
    public class AccessTokenModel : IAccessTokenModel
    {
        private readonly IAccessTokenRepository _repository;
        private readonly IAccessTokenUtility _utility;
        private readonly IConfiguration _configuration;

        public AccessTokenModel(IAccessTokenRepository repository, IAccessTokenUtility utility, IConfiguration configuration)
        {
            _repository = repository;
            _utility = utility;
            _configuration = configuration;
        }

        public ClaimsIdentity GetIdentity(string token)
        {
            return _utility.GetIdentity(token);
        }

        public string CreateAccessToken(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var claim = new Claim(ClaimTypes.Email , user.Email);

            return _utility.CreateAccessToken(new[] { claim });
        }
    }
}
