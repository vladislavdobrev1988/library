using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Exceptions;
using Library.Objects.Helpers.Constants;
using Library.Objects.Models.Implementations;
using Library.Objects.Proxies;
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
            var author = GetAuthor();

            author.FirstName = null;

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAuthorAsync(author));

            Assert.AreEqual(AuthorModel.ErrorMessage.FIRST_NAME_REQUIRED, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateAuthorAsync_EmptyFirstName_ThrowsException()
        {
            var author = GetAuthor();

            author.FirstName = " ";

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAuthorAsync(author));

            Assert.AreEqual(AuthorModel.ErrorMessage.FIRST_NAME_REQUIRED, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateAuthorAsync_NullLastName_ThrowsException()
        {
            var author = GetAuthor();

            author.LastName = null;

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAuthorAsync(author));

            Assert.AreEqual(AuthorModel.ErrorMessage.LAST_NAME_REQUIRED, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateAuthorAsync_EmptyLastName_ThrowsException()
        {
            var author = GetAuthor();

            author.LastName = " ";

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAuthorAsync(author));

            Assert.AreEqual(AuthorModel.ErrorMessage.LAST_NAME_REQUIRED, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateAuthorAsync_NullNobelPrize_ThrowsException()
        {
            var author = GetAuthor();

            author.NobelPrize = null;

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAuthorAsync(author));

            Assert.AreEqual(AuthorModel.ErrorMessage.NOBEL_PRIZE_REQUIRED, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateAuthorAsync_InvalidDateOfBirth_ThrowsException()
        {
            var author = GetAuthor();

            var dateOfBirthErrorMessage = "some date error";

            _dateValidatorMock
                .Setup(x => x.Validate(author.DateOfBirth))
                .Returns(dateOfBirthErrorMessage);

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAuthorAsync(author));

            Assert.AreEqual(dateOfBirthErrorMessage, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateAuthorAsync_InvalidDateOfDeath_ThrowsException()
        {
            var author = GetAuthor();

            var dateOfDeathErrorMessage = "some date error";

            _dateValidatorMock
                .Setup(x => x.Validate(author.DateOfDeath))
                .Returns(dateOfDeathErrorMessage);

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAuthorAsync(author));

            Assert.AreEqual(dateOfDeathErrorMessage, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateAuthorAsync_AlreadyExists_ThrowsException()
        {
            var author = GetAuthor();

            _repositoryMock
                .Setup(x => x.GetByNameAsync(author.FirstName, author.LastName))
                .Returns(Task.FromResult(new Author { Id = 1 }));

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAuthorAsync(author));

            Assert.AreEqual(AuthorModel.ErrorMessage.AUTHOR_EXISTS, ex.Message);
            Assert.AreEqual(HttpStatusCode.CONFLICT, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateAuthorAsync_ValidInput_WorksAsExpected()
        {
            const int ID = 5;

            var author = GetAuthor();

            Expression<Func<Author, bool>> expected = a =>
                a.FirstName == author.FirstName &&
                a.LastName == author.LastName &&
                a.NobelPrize == author.NobelPrize &&
                a.DateOfBirth == DateTime.Parse(author.DateOfBirth) &&
                a.DateOfDeath == DateTime.Parse(author.DateOfDeath);

            _repositoryMock
                .Setup(x => x.AddAsync(It.Is(expected)))
                .Returns(Task.FromResult(ID));

            var response = await _model.CreateAuthorAsync(author);

            Assert.IsNotNull(response);
            Assert.AreEqual(ID, response.Id);
        }
        
        [TestMethod]
        public async Task UpdateAuthorAsync_NullAuthor_ThrowsException()
        {
            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.UpdateAuthorAsync(1, null));

            Assert.AreEqual(AuthorModel.ErrorMessage.AUTHOR_REQUIRED, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task UpdateAuthorAsync_AuthorNotFound_ThrowsException()
        {
            const int UPDATED_AUTHOR_ID = 5;

            var author = GetAuthor();

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.UpdateAuthorAsync(UPDATED_AUTHOR_ID, author));

            var expectedMessage = string.Format(AuthorModel.ErrorMessage.NOT_FOUND_FORMAT, UPDATED_AUTHOR_ID);

            Assert.AreEqual(expectedMessage, ex.Message);
            Assert.AreEqual(HttpStatusCode.NOT_FOUND, ex.StatusCode);
        }

        [TestMethod]
        public async Task UpdateAuthorAsync_AlreadyExists_ThrowsException()
        {
            const int UPDATED_AUTHOR_ID = 5;
            const int DUPLICATE_AUTHOR_ID = 6;

            var author = GetAuthor();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(UPDATED_AUTHOR_ID))
                .Returns(Task.FromResult(new Author { Id = UPDATED_AUTHOR_ID }));

            _repositoryMock
                .Setup(x => x.GetByNameAsync(author.FirstName, author.LastName))
                .Returns(Task.FromResult(new Author { Id = DUPLICATE_AUTHOR_ID }));

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.UpdateAuthorAsync(UPDATED_AUTHOR_ID, author));

            Assert.AreEqual(AuthorModel.ErrorMessage.AUTHOR_EXISTS, ex.Message);
            Assert.AreEqual(HttpStatusCode.CONFLICT, ex.StatusCode);
        }

        [TestMethod]
        public async Task UpdateAuthorAsync_ValidInput_WorksAsExpected()
        {
            const int UPDATED_AUTHOR_ID = 5;

            var author = GetAuthor();
            author.DateOfDeath = null;

            var entity = new Author { Id = UPDATED_AUTHOR_ID };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(UPDATED_AUTHOR_ID))
                .Returns(Task.FromResult(entity));

            await _model.UpdateAuthorAsync(UPDATED_AUTHOR_ID, author);

            Assert.AreEqual(author.FirstName, entity.FirstName);
            Assert.AreEqual(author.LastName, entity.LastName);
            Assert.AreEqual(author.NobelPrize, entity.NobelPrize);
            Assert.AreEqual(DateTime.Parse(author.DateOfBirth), entity.DateOfBirth);
            Assert.IsNull(entity.DateOfDeath);

            _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        private AuthorProxy GetAuthor()
        {
            return new AuthorProxy
            {
                FirstName = "William",
                LastName = "Shakespeare",
                DateOfBirth = "1564-04-26",
                DateOfDeath = "1616-04-23",
                NobelPrize = false
            };
        }
    }
}
