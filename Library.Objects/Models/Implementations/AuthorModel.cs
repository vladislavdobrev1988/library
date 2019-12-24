using System;
using System.Globalization;
using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Models.Interfaces;
using Library.Objects.Proxies;
using Library.Objects.Repositories.Interfaces;

namespace Library.Objects.Models.Implementations
{
    public class AuthorModel : IAuthorModel
    {
        private readonly IAuthorRepository _repository;

        public AuthorModel(IAuthorRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> CreateAuthorAsync(AuthorProxy author)
        {
            // validate

            var entity = MapToEntity(author);

            _repository.Add(entity);

            await _repository.SaveChangesAsync();

            return entity.Id;
        }

        private Author MapToEntity(AuthorProxy author)
        {
            return new Author
            {
                FirstName = author.FirstName,
                LastName = author.LastName,
                DateOfBirth = DateTime.ParseExact(author.DateOfBirth, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                DateOfDeath = !string.IsNullOrWhiteSpace(author.DateOfDeath) ? DateTime.Parse(author.DateOfDeath) : (DateTime?)null
            };
        }
    }
}
