using System.Threading.Tasks;
using Library.Objects.Exceptions;
using Library.Objects.Helpers.Constants;
using Library.Objects.Models.Implementations;
using Library.Objects.Repositories.Interfaces;
using Library.Objects.Validation.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Library.Tests.ModelTests
{
    [TestClass]
    public class AuthorModelTests
    {
        private readonly Mock<IAuthorRepository> _repositoryMock;
        private readonly Mock<IDateValidator> _dateValidatorMock;
        private readonly Mock<IPageValidator> _pageValidatorMock;

        private readonly AuthorModel _model;

        public AuthorModelTests()
        {
            _repositoryMock = new Mock<IAuthorRepository>();
            _dateValidatorMock = new Mock<IDateValidator>();
            _pageValidatorMock = new Mock<IPageValidator>();

            _model = new AuthorModel(_repositoryMock.Object, _dateValidatorMock.Object, _pageValidatorMock.Object);
        }

        [TestMethod]
        public async Task CreateAuthorAsync_NullAuthor_ThrowsException()
        {
            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAuthorAsync(null));

            Assert.AreEqual(AuthorModel.ErrorMessage.AUTHOR_REQUIRED, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateAuthorAsync_NullFirstName_ThrowsException()
        {

        }

        [TestMethod]
        public async Task CreateAuthorAsync_EmptyFirstName_ThrowsException()
        {

        }

        [TestMethod]
        public async Task CreateAuthorAsync_NullLastName_ThrowsException()
        {

        }

        [TestMethod]
        public async Task CreateAuthorAsync_EmptyLastName_ThrowsException()
        {

        }

        [TestMethod]
        public async Task CreateAuthorAsync_NullNobelPrize_ThrowsException()
        {

        }

        [TestMethod]
        public async Task CreateAuthorAsync_InvalidDateOfBirth_ThrowsException()
        {

        }

        [TestMethod]
        public async Task CreateAuthorAsync_InvalidDateOfDeath_ThrowsException()
        {

        }

        [TestMethod]
        public async Task CreateAuthorAsync_AlreadyExists_ThrowsException()
        {

        }

        [TestMethod]
        public async Task CreateAuthorAsync_ValidInput_WorksAsExpected()
        {

        }
        
        [TestMethod]
        public async Task UpdateAuthorAsync_NullAuthor_ThrowsException()
        {

        }

        [TestMethod]
        public async Task UpdateAuthorAsync_AuthorNotFound_ThrowsException()
        {

        }

        [TestMethod]
        public async Task UpdateAuthorAsync_AlreadyExists_ThrowsException()
        {

        }

        [TestMethod]
        public async Task UpdateAuthorAsync_ValidInput_WorksAsExpected()
        {

        }
    }
}
