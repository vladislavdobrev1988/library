using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Exceptions;
using Library.Objects.Helpers.Constants;
using Library.Objects.Models.Implementations;
using Library.Objects.Models.Interfaces;
using Library.Objects.Proxies;
using Library.Objects.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Library.Tests.ModelTests
{
    [TestClass]
    public class AccountModelTests
    {
        private readonly Mock<IUserModel> _userModelMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly Mock<IAccessTokenManager> _accessTokenManagerMock;

        private readonly AccountModel _model;

        public AccountModelTests()
        {
            _userModelMock = new Mock<IUserModel>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();
            _accessTokenManagerMock = new Mock<IAccessTokenManager>();

            _model = new AccountModel(_userModelMock.Object, _passwordHasherMock.Object, _accessTokenManagerMock.Object);
        }

        [TestMethod]
        public async Task LogInAsync_NullCredentials_ThrowsException()
        {
            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.LogInAsync(null));

            Assert.AreEqual(AccountModel.ErrorMessage.CREDENTIALS_REQUIRED, ex.Message);
        }

        [TestMethod]
        public async Task LogInAsync_NullEmail_ThrowsException()
        {
            var credentials = GetCredentials();
            credentials.Email = null;

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.LogInAsync(credentials));

            Assert.AreEqual(CommonErrorMessage.EMAIL_REQUIRED, ex.Message);
        }

        [TestMethod]
        public async Task LogInAsync_EmptyEmail_ThrowsException()
        {
            var credentials = GetCredentials();
            credentials.Email = " ";

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.LogInAsync(credentials));

            Assert.AreEqual(CommonErrorMessage.EMAIL_REQUIRED, ex.Message);
        }

        [TestMethod]
        public async Task LogInAsync_NullPassword_ThrowsException()
        {
            var credentials = GetCredentials();
            credentials.Password = null;

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.LogInAsync(credentials));

            Assert.AreEqual(CommonErrorMessage.PASSWORD_REQUIRED, ex.Message);
        }

        [TestMethod]
        public async Task LogInAsync_EmptyPassword_ThrowsException()
        {
            var credentials = GetCredentials();
            credentials.Password = " ";

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.LogInAsync(credentials));

            Assert.AreEqual(CommonErrorMessage.PASSWORD_REQUIRED, ex.Message);
        }

        [TestMethod]
        public async Task LogInAsync_UserNotFound_ThrowsException()
        {
            var credentials = GetCredentials();

            _userModelMock
                .Setup(x => x.GetByEmailAsync(credentials.Email))
                .Returns(Task.FromResult((User)null));

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.LogInAsync(credentials));

            Assert.AreEqual(AccountModel.ErrorMessage.CREDENTIAL_MISMATCH, ex.Message);
        }

        [TestMethod]
        public async Task LogInAsync_PasswordMismatch_ThrowsException()
        {
            var credentials = GetCredentials();

            var user = new User
            {
                Email = credentials.Email,
                PasswordHash = "JKQWV"
            };

            _userModelMock
                .Setup(x => x.GetByEmailAsync(credentials.Email))
                .Returns(Task.FromResult(user));

            _passwordHasherMock
                .Setup(x => x.VerifyHashedPassword(null, user.PasswordHash, credentials.Password))
                .Returns(PasswordVerificationResult.Failed);

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.LogInAsync(credentials));

            Assert.AreEqual(AccountModel.ErrorMessage.CREDENTIAL_MISMATCH, ex.Message);
        }

        [TestMethod]
        public async Task LogInAsync_ValidCredentials_WorksAsExpected()
        {
            const string TOKEN = "mqerv134fjqv";

            var credentials = GetCredentials();

            var user = new User
            {
                Email = credentials.Email,
                PasswordHash = "JKQWV"
            };

            _userModelMock
                .Setup(x => x.GetByEmailAsync(credentials.Email))
                .Returns(Task.FromResult(user));

            _passwordHasherMock
                .Setup(x => x.VerifyHashedPassword(null, user.PasswordHash, credentials.Password))
                .Returns(PasswordVerificationResult.Success);

            Expression<Func<IEnumerable<Claim>, bool>> expected = collection =>
                collection != null &&
                collection.Any(x => x.Type == ClaimTypes.Email && x.Value == user.Email);

            _accessTokenManagerMock
                .Setup(m => m.CreateAccessToken(It.Is(expected)))
                .Returns(TOKEN);

            var result = await _model.LogInAsync(credentials);

            Assert.IsNotNull(result);
            Assert.AreEqual(TOKEN, result.AccessToken);
        }

        private static CredentialProxy GetCredentials()
        {
            return new CredentialProxy
            {
                Email = "a@b.com",
                Password = "123"
            };
        }
    }
}
