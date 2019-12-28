using System;
using System.Linq;
using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Helpers.Common;
using Library.Objects.Helpers.Extensions;
using Library.Objects.Helpers.Request;
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
            public const string AUTHOR_HAS_BOOKS = "Unable to delete author because of book references";
            public const string NOBEL_PRIZE_REQUIRED = "Nobel prize boolean value is required";
        }

        private readonly IAuthorRepository _repository;
        private readonly IDateValidator _dateValidator;
        private readonly IPageValidator _pageValidator;

        public AuthorModel(IAuthorRepository repository, IDateValidator dateValidator, IPageValidator pageValidator)
        {
            _repository = repository;
            _dateValidator = dateValidator;
            _pageValidator = pageValidator;
        }

        public async Task<IdResponse> CreateAuthorAsync(AuthorProxy author)
        {
            Validate(author);

            await ValidateUniqueness(author.FirstName, author.LastName, null);

            var entity = new Author();

            MapToEntity(author, entity);

            var id = await _repository.AddAsync(entity);

            return new IdResponse(id);
        }

        public async Task UpdateAuthorAsync(int id, AuthorProxy author)
        {
            Validate(author);

            var entity = await GetByIdAsync(id);

            await ValidateUniqueness(author.FirstName, author.LastName, id);

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

            var bookCount = await _repository.GetBookCountAsync(id);
            if (bookCount > 0)
            {
                ThrowHttp.Conflict(ErrorMessage.AUTHOR_HAS_BOOKS);
            }

            await _repository.RemoveAsync(author);
        }

        public async Task<PageResponse<AuthorProxy>> GetAuthorPageAsync(PageRequest pageRequest)
        {
            var pageError = _pageValidator.Validate(pageRequest);
            if (pageError != null)
            {
                ThrowHttp.BadRequest(pageError);
            }

            var page = await _repository.GetPageAsync(pageRequest);
            var total = await _repository.GetCountAsync();

            return new PageResponse<AuthorProxy>(page.Select(MapToProxy).ToArray(), total);
        }

        public async Task ValidateExistingAuthorAsync(int id)
        {
            await GetByIdAsync(id);
        }

        #region private

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

        private async Task ValidateUniqueness(string firstName, string lastName, int? authorId)
        {
            var duplicate = await _repository.GetByNameAsync(firstName, lastName);

            if (duplicate != null && duplicate.Id != authorId)
            {
                ThrowHttp.Conflict(ErrorMessage.AUTHOR_EXISTS);
            }
        }

        private void MapToEntity(AuthorProxy proxy, Author entity)
        {
            var deceased = !string.IsNullOrWhiteSpace(proxy.DateOfDeath);

            entity.FirstName = proxy.FirstName;
            entity.LastName = proxy.LastName;
            entity.DateOfBirth = DateTime.Parse(proxy.DateOfBirth);
            entity.DateOfDeath = deceased ? DateTime.Parse(proxy.DateOfDeath) : (DateTime?)null;
            entity.NobelPrize = proxy.NobelPrize.Value;
        }

        private AuthorProxy MapToProxy(Author author)
        {
            return new AuthorProxy
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                DateOfBirth = author.DateOfBirth.ToIsoDateString(),
                DateOfDeath = author.DateOfDeath.HasValue ? author.DateOfDeath.Value.ToIsoDateString() : null,
                NobelPrize = author.NobelPrize
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

            if (!author.NobelPrize.HasValue)
            {
                ThrowHttp.BadRequest(ErrorMessage.NOBEL_PRIZE_REQUIRED);
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

        #endregion
    }
}
