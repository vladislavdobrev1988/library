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

            var entity = MapToEntity(author);

            var id = await _repository.AddAsync(entity);

            return new IdResponse(id);
        }

        public async Task<AuthorProxy> GetAuthorByIdAsync(int id)
        {
            var author = await GetByIdAsync(id);

            return MapToProxy(author);
        }

        private async Task<Author> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                var errorMessage = string.Format(ErrorMessage.NOT_FOUND_FORMAT, id);
                ThrowHttp.NotFound(errorMessage);
            }

            return entity;
        }

        private Author MapToEntity(AuthorProxy author)
        {
            var deceased = !string.IsNullOrWhiteSpace(author.DateOfDeath);

            return new Author
            {
                FirstName = author.FirstName,
                LastName = author.LastName,
                DateOfBirth = DateTime.Parse(author.DateOfBirth),
                DateOfDeath = deceased ? DateTime.Parse(author.DateOfDeath) : (DateTime?)null
            };
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
