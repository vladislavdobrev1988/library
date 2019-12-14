﻿using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Helpers.Response;
using Library.Objects.Models.Base;
using Library.Objects.Models.Interfaces;
using Library.Objects.Proxies;
using Library.Objects.Validation;
using Microsoft.AspNetCore.Identity;

namespace Library.Objects.Models.Implementations
{
    public class AccountModel : BaseModel, IAccountModel
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
                ThrowHttpUnauthorized(ErrorMessage.CREDENTIAL_MISMATCH);
            }

            var token = await _accessTokenModel.CreateAsync(user);

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

        private bool HasPasswordMatch(string hashedPassword, string password)
        {
            return _passwordHasher.VerifyHashedPassword(null, hashedPassword, password) == PasswordVerificationResult.Success;
        }
    }
}
