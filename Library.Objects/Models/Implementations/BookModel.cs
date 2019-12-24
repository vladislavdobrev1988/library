using System;
using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Helpers.Response;
using Library.Objects.Models.Interfaces;
using Library.Objects.Proxies;
using Library.Objects.Repositories.Interfaces;

namespace Library.Objects.Models.Implementations
{
    public class BookModel : IBookModel
    {
        private readonly IBookRepository _repository;

        public BookModel(IBookRepository repository)
        {
            _repository = repository;
        }

        public async Task<IdResponse> CreateBookAsync(BookProxy book)
        {
            Validate(book);

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

        }
    }
}
