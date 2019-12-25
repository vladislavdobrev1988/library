using System;
using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Helpers.Common;
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

            await _authorModel.GetAuthorByIdAsync(book.AuthorId);

            var entity = MapToEntity(book);

            var id = await _repository.AddAsync(entity);

            return new IdResponse(id);
        }

        private Book MapToEntity(BookProxy book)
        {
            return new Book
            {
                Title = book.Title,
                ReleaseDate = DateTime.Parse(book.ReleaseDate),
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

            var dateError = _dateValidator.Validate(book.ReleaseDate);
            if (dateError != null)
            {
                ThrowHttp.BadRequest(dateError);
            }
        }
    }
}
