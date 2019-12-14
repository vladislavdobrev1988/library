using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Helpers.Common;
using Library.Objects.Helpers.Response;
using Library.Objects.Models.Interfaces;
using Library.Objects.Proxies;
using Library.Objects.Validation;
using Microsoft.AspNetCore.Identity;

namespace Library.Objects.Models.Implementations
{
    public class AccountModel : IAccountModel
    {
        private readonly IUserModel _userModel;
        private readonly IAccessTokenModel _accessTokenModel;
        private readonly IPasswordHasher<User> _passwordHasher;

        private static class ErrorMessage
        {
            public const string CREDENTIALS_REQUIRED = "Credentials object is required";
            public const string CREDENTIAL_MISMATCH = "Email or password mismatch";
        }

        public AccountModel(IUserModel userModel, IAccessTokenModel accessTokenModel, IPasswordHasher<User> passwordHasher)
        {
            _userModel = userModel;
            _accessTokenModel = accessTokenModel;
            _passwordHasher = passwordHasher;
        }

        public async Task<AccessTokenResponse> LogIn(CredentialProxy credentials)
        {
            ValidateCredentials(credentials);

            var user = await _userModel.GetByEmailAsync(credentials.Email);

            if (user == null || !HasPasswordMatch(user.PasswordHash, credentials.Password))
            {
                ThrowHttp.Unauthorized(ErrorMessage.CREDENTIAL_MISMATCH);
            }

            var token = await _accessTokenModel.CreateAccessTokenAsync(user);

            return new AccessTokenResponse(token);
        }

        private void ValidateCredentials(CredentialProxy credentials)
        {
            if (credentials == null)
            {
                ThrowHttp.BadRequest(ErrorMessage.CREDENTIALS_REQUIRED);
            }

            if (string.IsNullOrWhiteSpace(credentials.Email))
            {
                ThrowHttp.BadRequest(Email.ErrorMessage.REQUIRED);
            }

            if (string.IsNullOrWhiteSpace(credentials.Password))
            {
                ThrowHttp.BadRequest(Password.ErrorMessage.Required);
            }
        }

        private bool HasPasswordMatch(string hashedPassword, string password)
        {
            return _passwordHasher.VerifyHashedPassword(null, hashedPassword, password) == PasswordVerificationResult.Success;
        }
    }
}
