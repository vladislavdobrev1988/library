using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Library.Objects.Helpers.Constants;
using Library.Objects.Helpers.Response;
using Library.Objects.Models.Base;
using Library.Objects.Models.Interfaces;
using Library.Objects.Proxies;
using Library.Objects.Services.Interfaces;
using Library.Objects.Validation;
using Microsoft.Extensions.Configuration;

namespace Library.Objects.Models.Implementations
{
    public class AccountModel : BaseModel, IAccountModel
    {
        private const string VALIDITY_KEY = "AccessTokenValidityInMinutes";

        private readonly IConfiguration _configuration;
        private readonly IAccessTokenUtility _accessTokenUtility;

        private static class ErrorMessage
        {
            public const string CREDENTIALS_REQUIRED = "Credentials object is required";
        }

        public AccountModel(IConfiguration configuration, IAccessTokenUtility accessTokenUtility)
        {
            _configuration = configuration;
            _accessTokenUtility = accessTokenUtility;
        }

        public async Task<AccessTokenResponse> LogIn(CredentialProxy credentials)
        {
            ValidateCredentials(credentials);

            var validity = _configuration.GetValue<double>(VALIDITY_KEY);

            var expires = DateTime.UtcNow.AddMinutes(validity);

            var claim = new Claim(IdentityClaims.EMAIL, "pesho@pesho.com");

            var token = _accessTokenUtility.CreateAccessToken(expires, new[] { claim });

            return new AccessTokenResponse(token);
        }

        private void ValidateCredentials(CredentialProxy credentials)
        {
            if (credentials == null)
            {
                ThrowHttpBadRequest(ErrorMessage.CREDENTIALS_REQUIRED);
            }

            if (string.IsNullOrWhiteSpace(credentials.Email))
            {
                ThrowHttpBadRequest(Email.ErrorMessage.REQUIRED);
            }

            if (string.IsNullOrWhiteSpace(credentials.Password))
            {
                ThrowHttpBadRequest(Password.ErrorMessage.Required);
            }
        }
    }
}
