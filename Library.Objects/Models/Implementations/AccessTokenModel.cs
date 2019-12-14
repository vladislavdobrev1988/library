using System;
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
        private const string VALIDITY_KEY = "AccessTokenValidityInMinutes";

        private readonly IAccessTokenRepository _repository;
        private readonly IAccessTokenUtility _utility;
        private readonly IConfiguration _configuration;

        public AccessTokenModel(IAccessTokenRepository repository, IAccessTokenUtility utility, IConfiguration configuration)
        {
            _repository = repository;
            _utility = utility;
            _configuration = configuration;
        }

        public async Task<bool> IsValidAccessTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var entity = await _repository.GetByToken(token);
            if (entity == null)
            {
                return false;
            }

            if (entity.Expires < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }

        public async Task<string> CreateAccessTokenAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var validity = _configuration.GetValue<double>(VALIDITY_KEY);

            var expires = DateTime.UtcNow.AddMinutes(validity);

            var claim = new Claim(IdentityClaims.EMAIL, user.Email);

            var token = _utility.CreateAccessToken(expires, new[] { claim });

            var entity = new AccessToken
            {
                Token = token,
                Expires = expires,
                UserId = user.Id
            };

            _repository.Add(entity);

            await _repository.SaveChangesAsync();

            return token;
        }
    }
}
