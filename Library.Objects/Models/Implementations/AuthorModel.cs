using System;
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
    public class AuthorModel : IAuthorModel
    {
        private static class ErrorMessage
        {
            public const string AUTHOR_REQUIRED = "Author object is required";
            public const string FIRST_NAME_REQUIRED = "First name is required";
            public const string LAST_NAME_REQUIRED = "Last name is required";
            public const string AUTHOR_EXISTS = "Author with the same name already exists";
            public const string NOT_FOUND_FORMAT = "Author with id {0} was not found";
        }

        private readonly IAuthorRepository _repository;
        private readonly IDateValidator _dateValidator;

        public AuthorModel(IAuthorRepository repository, IDateValidator dateValidator)
        {
            _repository = repository;
            _dateValidator = dateValidator;
        }

        public async Task<IdResponse> CreateAuthorAsync(AuthorProxy author)
        {
            Validate(author);

            var existing = await _repository.GetByNameAsync(author.FirstName, author.LastName);
            if (existing != null)
            {
                ThrowHttp.Conflict(ErrorMessage.AUTHOR_EXISTS);
            }

            var entity = new Author();

            MapToEntity(author, entity);

            var id = await _repository.AddAsync(entity);

            return new IdResponse(id);
        }

        public async Task UpdateAuthorAsync(int id, AuthorProxy author)
        {
            Validate(author);

            var entity = await GetByIdAsync(id);

            var existing = await _repository.GetByNameAsync(author.FirstName, author.LastName);

            if (existing != null && existing.Id != entity.Id)
            {
                ThrowHttp.Conflict(ErrorMessage.AUTHOR_EXISTS);
            }

            MapToEntity(author, entity);

            await _repository.SaveChangesAsync();
        }

        public async Task<AuthorProxy> GetAuthorByIdAsync(int id)
        {
            var author = await GetByIdAsync(id);

            return MapToProxy(author);
        }

        public async Task DeleteAuthorAsync(int id)
        {
            var author = await GetByIdAsync(id);

            // validate has no books

            await _repository.RemoveAsync(author);
        }

        private async Task<Author> GetByIdAsync(int id)
        {
            var author = await _repository.GetByIdAsync(id);
            if (author == null)
            {
                var errorMessage = string.Format(ErrorMessage.NOT_FOUND_FORMAT, id);
                ThrowHttp.NotFound(errorMessage);
            }

            return author;
        }

        private void MapToEntity(AuthorProxy proxy, Author entity)
        {
            var deceased = !string.IsNullOrWhiteSpace(proxy.DateOfDeath);

            entity.FirstName = proxy.FirstName;
            entity.LastName = proxy.LastName;
            entity.DateOfBirth = DateTime.Parse(proxy.DateOfBirth);
            entity.DateOfDeath = deceased ? DateTime.Parse(proxy.DateOfDeath) : (DateTime?)null;

        }

        private AuthorProxy MapToProxy(Author author)
        {
            return new AuthorProxy
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                DateOfBirth = author.DateOfBirth.ToIsoDateString(),
                DateOfDeath = author.DateOfDeath.HasValue ? author.DateOfDeath.Value.ToIsoDateString() : null
            };
        }

        private void Validate(AuthorProxy author)
        {
            if (author == null)
            {
                ThrowHttp.BadRequest(ErrorMessage.AUTHOR_REQUIRED);
            }

            if (string.IsNullOrWhiteSpace(author.FirstName))
            {
                ThrowHttp.BadRequest(ErrorMessage.FIRST_NAME_REQUIRED);
            }

            if (string.IsNullOrWhiteSpace(author.LastName))
            {
                ThrowHttp.BadRequest(ErrorMessage.LAST_NAME_REQUIRED);
            }

            var dateOfBirthError = _dateValidator.Validate(author.DateOfBirth);
            if (dateOfBirthError != null)
            {
                ThrowHttp.BadRequest(dateOfBirthError);
            }

            if (string.IsNullOrWhiteSpace(author.DateOfDeath))
            {
                return;
            }

            var dateOfDeathError = _dateValidator.Validate(author.DateOfDeath);
            if (dateOfDeathError != null)
            {
                ThrowHttp.BadRequest(dateOfDeathError);
            }
        }
    }
}
