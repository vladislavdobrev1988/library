using System;
using System.Linq;
using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Helpers.Common;
using Library.Objects.Helpers.Extensions;
using Library.Objects.Helpers.Response;
using Library.Objects.Models.Interfaces;
using Library.Objects.Proxies;
using Library.Objects.Repositories.Interfaces;
using Library.Objects.Validation.Interfaces;

namespace Library.Objects.Models.Implementations
{
    public class BookModel : IBookModel
    {
        private static class ErrorMessage
        {
            public const string BOOK_REQUIRED = "Book object is required";
            public const string TITLE_REQUIRED = "Title is required";
            public const string NOT_FOUND_FORMAT = "Book with id {0} was not found";
            public const string BOOK_EXISTS = "Book with the same title already exists";
        }

        private readonly IBookRepository _repository;
        private readonly IAuthorModel _authorModel;
        private readonly IDateValidator _dateValidator;

        public BookModel(IBookRepository repository, IAuthorModel authorModel, IDateValidator dateValidator)
        {
            _repository = repository;
            _authorModel = authorModel;
            _dateValidator = dateValidator;
        }

        public async Task<IdResponse> CreateBookAsync(BookProxy book)
        {
            Validate(book);

            await _authorModel.ValidateExistingAuthorAsync(book.AuthorId);

            await ValidateUniqueness(book.Title, null);

            var entity = new Book();

            MapToEntity(book, entity);

            var id = await _repository.AddAsync(entity);

            return new IdResponse(id);
        }

        public async Task UpdateBookAsync(int id, BookProxy book)
        {
            Validate(book);

            var entity = await GetByIdAsync(id);

            await _authorModel.ValidateExistingAuthorAsync(book.AuthorId);

            await ValidateUniqueness(book.Title, id);

            MapToEntity(book, entity);

            await _repository.SaveChangesAsync();
        }

        public async Task<BookProxy> GetBookByIdAsync(int id)
        {
            var book = await GetByIdAsync(id);

            return MapToProxy(book);
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await GetByIdAsync(id);

            await _repository.RemoveAsync(book);
        }

        public async Task<BookProxy[]> GetBooksByAuthorAsync(int authorId)
        {
            await _authorModel.ValidateExistingAuthorAsync(authorId);

            var books = await _repository.GetByAuthorAsync(authorId);

            return books.Select(MapToProxy).ToArray();
        }

        #region private

        private async Task<Book> GetByIdAsync(int id)
        {
            var book = await _repository.GetByIdAsync(id);
            if (book == null)
            {
                var message = string.Format(ErrorMessage.NOT_FOUND_FORMAT, id);
                ThrowHttp.NotFound(message);
            }

            return book;
        }

        private async Task ValidateUniqueness(string title, int? bookId)
        {
            var duplicate = await _repository.GetByTitleAsync(title);

            if (duplicate != null && duplicate.Id != bookId)
            {
                ThrowHttp.Conflict(ErrorMessage.BOOK_EXISTS);
            }
        }

        private void MapToEntity(BookProxy proxy, Book entity)
        {
            entity.Title = proxy.Title;
            entity.PublishDate = DateTime.Parse(proxy.PublishDate);
            entity.AuthorId = proxy.AuthorId;
        }

        private BookProxy MapToProxy(Book book)
        {
            return new BookProxy
            {
                Id = book.Id,
                Title = book.Title,
                PublishDate = book.PublishDate.ToIsoDateString(),
                AuthorId = book.AuthorId
            };
        }

        private void Validate(BookProxy book)
        {
            if (book == null)
            {
                ThrowHttp.BadRequest(ErrorMessage.BOOK_REQUIRED);
            }

            if (string.IsNullOrWhiteSpace(book.Title))
            {
                ThrowHttp.BadRequest(ErrorMessage.TITLE_REQUIRED);
            }

            var dateError = _dateValidator.Validate(book.PublishDate);
            if (dateError != null)
            {
                ThrowHttp.BadRequest(dateError);
            }
        }

        #endregion
    }
}
