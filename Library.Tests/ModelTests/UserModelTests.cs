using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Exceptions;
using Library.Objects.Models.Implementations;
using Library.Objects.Proxies;
using Library.Objects.Repositories.Interfaces;
using Library.Objects.Validation.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Library.Tests.ModelTests
{
    [TestClass]
    public class UserModelTests
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly Mock<IEmailValidator> _emailValidatorMock;
        private readonly Mock<IPasswordValidator> _passwordValidatorMock;

        private readonly UserModel _model;
        
        public UserModelTests()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();
            _emailValidatorMock = new Mock<IEmailValidator>();
            _passwordValidatorMock = new Mock<IPasswordValidator>();

            _model = new UserModel(_repositoryMock.Object, _passwordHasherMock.Object, _emailValidatorMock.Object, _passwordValidatorMock.Object);
        }

        [TestMethod]
        public async Task CreateUserAsync_NullUser_ThrowsAsExpected()
        {
            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateUserAsync(null));

            Assert.AreEqual(UserModel.ErrorMessage.USER_REQUIRED, ex.Message);
        }

        [TestMethod]
        public async Task CreateUserAsync_InvalidEmail_ThrowsAsExpected()
        {
            const string ERROR_MESSAGE = "some";

            var user = GetUser();

            _emailValidatorMock.Setup(x => x.Validate(user.Email)).Returns(ERROR_MESSAGE);

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateUserAsync(user));

            Assert.AreEqual(ERROR_MESSAGE, ex.Message);
        }

        [TestMethod]
        public async Task CreateUserAsync_InvalidPassword_ThrowsAsExpected()
        {
            const string ERROR_MESSAGE = "some";

            var user = GetUser();

            _passwordValidatorMock.Setup(x => x.Validate(user.Password)).Returns(ERROR_MESSAGE);

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateUserAsync(user));

            Assert.AreEqual(ERROR_MESSAGE, ex.Message);
        }

        [TestMethod]
        public async Task CreateUserAsync_EmptyFirstName_ThrowsAsExpected()
        {
            var user = GetUser();

            user.FirstName = " ";

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateUserAsync(user));

            Assert.AreEqual(UserModel.ErrorMessage.FIRST_NAME_REQUIRED, ex.Message);
        }

        [TestMethod]
        public async Task CreateUserAsync_EmptyLastName_ThrowsAsExpected()
        {
            var user = GetUser();

            user.LastName = " ";

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateUserAsync(user));

            Assert.AreEqual(UserModel.ErrorMessage.LAST_NAME_REQUIRED, ex.Message);
        }

        [TestMethod]
        public async Task CreateUserAsync_ExistingEmail_ThrowsAsExpected()
        {
            var user = GetUser();

            _repositoryMock.Setup(x => x.GetByEmail(user.Email)).Returns(Task.FromResult(new User()));

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateUserAsync(user));

            Assert.AreEqual(UserModel.ErrorMessage.EMAIL_EXISTS, ex.Message);
        }

        [TestMethod]
        public async Task CreateUserAsync_ValidInput_WorksAsExpected()
        {
            var user = GetUser();

            var passwordHash = "FKkcq93jrfKFQj203fj==";

            _passwordHasherMock.Setup(x => x.HashPassword(null, user.Password)).Returns(passwordHash);

            await _model.CreateUserAsync(user);

            Expression<Func<User, bool>> expected = u =>
               u.FirstName == user.FirstName &&
               u.LastName == user.LastName &&
               u.Email == user.Email &&
               u.PasswordHash == passwordHash;

            _repositoryMock.Verify(x => x.Add(It.Is(expected)), Times.Once);
            _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        private UserProxy GetUser()
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
