using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Exceptions;
using Library.Objects.Models.Implementations;
using Library.Objects.Models.Interfaces;
using Library.Objects.Proxies;
using Library.Objects.Repositories.Interfaces;
using Library.Objects.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Library.Tests.ModelTests
{
    [TestClass]
    public class UserModelTests
    {
        private Mock<IUserRepository> _repositoryMock;
        private Mock<IPasswordHasher<User>> _passwordHasherMock;

        private IUserModel _model;

        [TestInitialize]
        public void Init()
        {
            _repositoryMock = new Mock<IUserRepository>(/*MockBehavior.Strict*/);
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();

            _model = new UserModel(_repositoryMock.Object, _passwordHasherMock.Object);
        }

        [TestMethod]
        public async Task CreateAsync_NullUser_ThrowsAsExpected()
        {
            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAsync(null));

            Assert.AreEqual(UserModel.ErrorMessage.USER_REQUIRED, ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_EmptyEmail_ThrowsAsExpected()
        {
            var user = GetValidUser();

            user.Email = " ";

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAsync(user));

            Assert.AreEqual(Email.ErrorMessage.REQUIRED, ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_InvalidEmail_ThrowsAsExpected()
        {
            var user = GetValidUser();

            user.Email = "x";

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAsync(user));

            var expectedMessage = string.Format(Email.ErrorMessage.INVALID_EMAIL_FORMAT, user.Email);

            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_EmptyPassword_ThrowsAsExpected()
        {
            var user = GetValidUser();

            user.Password = " ";

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAsync(user));

            Assert.AreEqual(Password.ErrorMessage.Required, ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_InvalidPassword_ThrowsAsExpected()
        {
            var user = GetValidUser();

            user.Password = "@bC4";

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAsync(user));

            Assert.AreEqual(Password.ErrorMessage.MinCharacterCount, ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_EmptyFirstName_ThrowsAsExpected()
        {
            var user = GetValidUser();

            user.FirstName = " ";

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAsync(user));

            Assert.AreEqual(UserModel.ErrorMessage.FIRST_NAME_REQUIRED, ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_EmptyLastName_ThrowsAsExpected()
        {
            var user = GetValidUser();

            user.LastName = " ";

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAsync(user));

            Assert.AreEqual(UserModel.ErrorMessage.LAST_NAME_REQUIRED, ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_ExistingEmail_ThrowsAsExpected()
        {
            var user = GetValidUser();

            _repositoryMock.Setup(x => x.GetByEmail(user.Email)).Returns(Task.FromResult(new User()));

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAsync(user));

            Assert.AreEqual(UserModel.ErrorMessage.EMAIL_EXISTS, ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_ValidInput_WorksAsExpected()
        {
            var user = GetValidUser();

            var passwordHash = "FKkcq93jrfKFQj203fj==";

            _passwordHasherMock.Setup(x => x.HashPassword(null, user.Password)).Returns(passwordHash);

            await _model.CreateAsync(user);

            Expression<Func<User, bool>> expected = u =>
               u.FirstName == user.FirstName &&
               u.LastName == user.LastName &&
               u.Email == user.Email &&
               u.PasswordHash == passwordHash;

            _repositoryMock.Verify(x => x.Add(It.Is(expected)), Times.Once);
            _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        private UserProxy GetValidUser()
        {
            return new UserProxy
            {
                FirstName = "Pesho",
                LastName = "Petrov",
                Email = "pe@sho.com",
                Password = "1q2w3e$R"
            };
        }
    }
}
