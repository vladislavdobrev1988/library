using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Exceptions;
using Library.Objects.Helpers.Constants;
using Library.Objects.Models.Implementations;
using Library.Objects.Models.Interfaces;
using Library.Objects.Proxies;
using Library.Objects.Repositories.Interfaces;
using Library.Objects.Validation.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Library.Tests.ModelTests
{
    [TestClass]
    public class BookModelTests
    {
        private const string TITLE = "Catch-22";
        private const string PUBLISH_DATE = "1961-11-10";

        private readonly Mock<IBookRepository> _repositoryMock;
        private readonly Mock<IAuthorModel> _authorModelMock;
        private readonly Mock<IDateValidator> _dateValidatorMock;

        private readonly BookModel _model;

        public BookModelTests()
        {
            _repositoryMock = new Mock<IBookRepository>();
            _authorModelMock = new Mock<IAuthorModel>();
            _dateValidatorMock = new Mock<IDateValidator>();

            _model = new BookModel(_repositoryMock.Object, _authorModelMock.Object, _dateValidatorMock.Object);
        }

        [TestMethod]
        public async Task CreateBookAsync_NullBook_ThrowsException()
        {
            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateBookAsync(null));

            Assert.AreEqual(BookModel.ErrorMessage.BOOK_REQUIRED, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateBookAsync_NullTitle_ThrowsException()
        {
            var book = GetProxy();
            book.Title = null;

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateBookAsync(book));

            Assert.AreEqual(BookModel.ErrorMessage.TITLE_REQUIRED, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateBookAsync_EmptyTitle_ThrowsException()
        {
            var book = GetProxy();
            book.Title = " ";

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateBookAsync(book));

            Assert.AreEqual(BookModel.ErrorMessage.TITLE_REQUIRED, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateBookAsync_InvalidPublishDate_ThrowsException()
        {
            var book = GetProxy();

            var invalidPublishDateMessage = "some";

            _dateValidatorMock
                .Setup(x => x.Validate(book.PublishDate))
                .Returns(invalidPublishDateMessage);

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateBookAsync(book));

            Assert.AreEqual(invalidPublishDateMessage, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateBookAsync_AuthorDoesNotExist_ThrowsException()
        {
            var book = GetProxy();

            var authorNotFoundException = new HttpResponseException(HttpStatusCode.NOT_FOUND, "not found");

            _authorModelMock
                .Setup(x => x.ValidateExistingAuthorAsync(book.AuthorId))
                .Throws(authorNotFoundException);

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateBookAsync(book));

            Assert.AreEqual(authorNotFoundException.Message, ex.Message);
            Assert.AreEqual(HttpStatusCode.NOT_FOUND, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateBookAsync_BookAlreadyExists_ThrowsException()
        {
            var book = GetProxy();

            _repositoryMock
                .Setup(x => x.GetByTitleAsync(book.Title)).
                Returns(Task.FromResult(new Book { Id = 1 }));

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.CreateBookAsync(book));

            Assert.AreEqual(BookModel.ErrorMessage.BOOK_EXISTS, ex.Message);
            Assert.AreEqual(HttpStatusCode.CONFLICT, ex.StatusCode);
        }

        [TestMethod]
        public async Task CreateBookAsync_ValidInput_WorksAsExpected()
        {
            const int ID = 5;

            var book = GetProxy();
            
            Expression<Func<Book, bool>> expected = b =>
                b.Title == book.Title &&
                b.PublishDate == DateTime.Parse(book.PublishDate) &&
                b.AuthorId == book.AuthorId;

            _repositoryMock
                .Setup(x => x.AddAsync(It.Is(expected)))
                .Returns(Task.FromResult(ID));

            var result = await _model.CreateBookAsync(book);

            Assert.IsNotNull(result);
            Assert.AreEqual(ID, result.Id);
        }

        [TestMethod]
        public async Task UpdateBookAsync_NullBook_ThrowsException()
        {
            const int UPDATED_BOOK_ID = 1;

            var ex = await Assert.ThrowsExceptionAsync<HttpResponseException>(async () => await _model.UpdateBookAsync(UPDATED_BOOK_ID, null));

            Assert.AreEqual(BookModel.ErrorMessage.BOOK_REQUIRED, ex.Message);
            Assert.AreEqual(HttpStatusCode.BAD_REQUEST, ex.StatusCode);
        }

        [TestMethod]
        public async Task UpdateBookAsync_BookNotFound_ThrowsException()
        {

        }

        [TestMethod]
        public async Task UpdateBookAsync_AuthorDoesNotExist_ThrowsException()
        {

        }

        [TestMethod]
        public async Task UpdateBookAsync_BookAlreadyExists_ThrowsException()
        {

        }

        [TestMethod]
        public async Task UpdateBookAsync_ValidInput_WorksAsExpected()
        {

        }

        private BookProxy GetProxy()
        {
            return new BookProxy
            {
                Title = TITLE,
                PublishDate = PUBLISH_DATE,
                AuthorId = 1
            };
        }

        private Book GetEntity()
        {
            return new Book
            {
                Id = 1,
                Title = TITLE,
                PublishDate = DateTime.Parse(PUBLISH_DATE),
                AuthorId = 1
            };
        }
    }
}
