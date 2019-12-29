using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Exceptions;
using Library.Objects.Helpers.Constants;
using Library.Objects.Helpers.Request;
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
        private const string FIRST_NAME = "William";
        private const string LAST_NAME = "Shakespeare";
        private const string DATE_OF_BIRTH = "1564-04-26";
        private const string DATE_OF_DEATH = "1616-04-23";

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
            var author = GetProxy();

            author.FirstName = null;

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAuthorAsync(author));

            Assert.AreEqual(AuthorModel.ErrorMessage.FIRST_NAME_REQUIRED, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateAuthorAsync_EmptyFirstName_ThrowsException()
        {
            var author = GetProxy();

            author.FirstName = " ";

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAuthorAsync(author));

            Assert.AreEqual(AuthorModel.ErrorMessage.FIRST_NAME_REQUIRED, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateAuthorAsync_NullLastName_ThrowsException()
        {
            var author = GetProxy();

            author.LastName = null;

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAuthorAsync(author));

            Assert.AreEqual(AuthorModel.ErrorMessage.LAST_NAME_REQUIRED, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateAuthorAsync_EmptyLastName_ThrowsException()
        {
            var author = GetProxy();

            author.LastName = " ";

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAuthorAsync(author));

            Assert.AreEqual(AuthorModel.ErrorMessage.LAST_NAME_REQUIRED, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateAuthorAsync_NullNobelPrize_ThrowsException()
        {
            var author = GetProxy();

            author.NobelPrize = null;

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateAuthorAsync(author));

            Assert.AreEqual(AuthorModel.ErrorMessage.NOBEL_PRIZE_REQUIRED, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateAuthorAsync_InvalidDateOfBirth_ThrowsException()
        {
            var author = GetProxy();

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
            var author = GetProxy();

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
            var author = GetProxy();

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

            var author = GetProxy();

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

            var author = GetProxy();

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

            var author = GetProxy();

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

            var author = GetProxy();
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

        [TestMethod]
        public async Task GetAuthorByIdAsync_AuthorNotFound_ThrowsException()
        {
            const int ID = 5;

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.GetAuthorByIdAsync(ID));

            var expectedMessage = string.Format(AuthorModel.ErrorMessage.NOT_FOUND_FORMAT, ID);

            Assert.AreEqual(expectedMessage, ex.Message);
            Assert.AreEqual(HttpStatusCode.NOT_FOUND, ex.StatusCode);
        }

        [TestMethod]
        public async Task GetAuthorByIdAsync_AuthorFound_ReturnsExpectedResult()
        {
            var entity = GetEntity();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(entity.Id))
                .Returns(Task.FromResult(entity));

            var proxy = await _model.GetAuthorByIdAsync(entity.Id);

            Assert.IsNotNull(proxy);
            Assert.AreEqual(entity.Id, proxy.Id);
            Assert.AreEqual(entity.FirstName, proxy.FirstName);
            Assert.AreEqual(entity.LastName, proxy.LastName);
            Assert.AreEqual(entity.NobelPrize, proxy.NobelPrize);
            Assert.AreEqual(entity.DateOfBirth.ToString(DateFormat.ISO_8601), proxy.DateOfBirth);
            Assert.AreEqual(entity.DateOfDeath.Value.ToString(DateFormat.ISO_8601), proxy.DateOfDeath);
        }

        [TestMethod]
        public async Task DeleteAuthorByIdAsync_AuthorNotFound_ThrowsException()
        {
            const int ID = 5;

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.DeleteAuthorAsync(ID));

            var expectedMessage = string.Format(AuthorModel.ErrorMessage.NOT_FOUND_FORMAT, ID);

            Assert.AreEqual(expectedMessage, ex.Message);
            Assert.AreEqual(HttpStatusCode.NOT_FOUND, ex.StatusCode);
        }

        [TestMethod]
        public async Task DeleteAuthorByIdAsync_AuthorHasBooks_ThrowsException()
        {
            var entity = GetEntity();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(entity.Id))
                .Returns(Task.FromResult(entity));

            _repositoryMock
                .Setup(x => x.GetBookCountAsync(entity.Id))
                .Returns(Task.FromResult(1));

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.DeleteAuthorAsync(entity.Id));

            Assert.AreEqual(AuthorModel.ErrorMessage.AUTHOR_HAS_BOOKS, ex.Message);
            Assert.AreEqual(HttpStatusCode.CONFLICT, ex.StatusCode);
        }

        [TestMethod]
        public async Task DeleteAuthorByIdAsync_AuthorFound_WorksAsExpected()
        {
            var entity = GetEntity();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(entity.Id))
                .Returns(Task.FromResult(entity));

            await _model.DeleteAuthorAsync(entity.Id);

            _repositoryMock.Verify(x => x.RemoveAsync(entity), Times.Once);
        }

        [TestMethod]
        public async Task GetAuthorPageAsync_InvalidRequest_ThrowsException()
        {
            var pageRequest = new PageRequest();

            var pageRequestError = "some";

            _pageValidatorMock
                .Setup(x => x.Validate(pageRequest))
                .Returns(pageRequestError);

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.GetAuthorPageAsync(pageRequest));

            Assert.AreEqual(pageRequestError, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task GetAuthorPageAsync_ValidRequest_ReturnExpectedResult()
        {
            const int TOTAL = 1;
            var entity = GetEntity();
            var page = new[] { entity };

            var pageRequest = new PageRequest
            {
                Page = 1,
                Size = 10
            };

            _repositoryMock
                .Setup(x => x.GetPageAsync(pageRequest))
                .Returns(Task.FromResult(page));

            _repositoryMock
                .Setup(x => x.GetCountAsync())
                .Returns(Task.FromResult(TOTAL));

            var result = await _model.GetAuthorPageAsync(pageRequest);

            Assert.IsNotNull(result);
            Assert.AreEqual(TOTAL, result.Total);

            Assert.IsNotNull(result.Page);
            Assert.AreEqual(page.Length, result.Page.Length);

            var proxy = result.Page.Single();

            Assert.AreEqual(entity.Id, proxy.Id);
            Assert.AreEqual(entity.FirstName, proxy.FirstName);
            Assert.AreEqual(entity.LastName, proxy.LastName);
            Assert.AreEqual(entity.NobelPrize, proxy.NobelPrize);
            Assert.AreEqual(entity.DateOfBirth.ToString(DateFormat.ISO_8601), proxy.DateOfBirth);
            Assert.AreEqual(entity.DateOfDeath.Value.ToString(DateFormat.ISO_8601), proxy.DateOfDeath);
        }

        [TestMethod]
        public async Task ValidateExistingAuthorAsync_AuthorNotFound_ThrowsException()
        {
            const int ID = 5;

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.ValidateExistingAuthorAsync(ID));

            var expectedMessage = string.Format(AuthorModel.ErrorMessage.NOT_FOUND_FORMAT, ID);

            Assert.AreEqual(expectedMessage, ex.Message);
            Assert.AreEqual(HttpStatusCode.NOT_FOUND, ex.StatusCode);
        }

        [TestMethod]
        public async Task ValidateExistingAuthorAsync_AuthorFound_DoesNotThrowException()
        {
            var entity = GetEntity();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(entity.Id))
                .Returns(Task.FromResult(entity));

            await _model.ValidateExistingAuthorAsync(entity.Id);
        }

        private AuthorProxy GetProxy()
        {
            return new AuthorProxy
            {
                FirstName = FIRST_NAME,
                LastName = LAST_NAME,
                DateOfBirth = DATE_OF_BIRTH,
                DateOfDeath = DATE_OF_DEATH,
                NobelPrize = false
            };
        }

        private Author GetEntity()
        {
            return new Author
            {
                Id = 1,
                FirstName = FIRST_NAME,
                LastName = LAST_NAME,
                DateOfBirth = DateTime.Parse(DATE_OF_BIRTH),
                DateOfDeath = DateTime.Parse(DATE_OF_DEATH),
                NobelPrize = false
            };
        }
    }
}
