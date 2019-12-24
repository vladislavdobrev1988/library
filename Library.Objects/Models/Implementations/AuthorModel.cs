﻿using System;
using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Helpers.Common;
using Library.Objects.Models.Interfaces;
using Library.Objects.Proxies;
using Library.Objects.Repositories.Interfaces;
using Library.Objects.Validation.Interfaces;

namespace Library.Objects.Models.Implementations
{
    public class AuthorModel : IAuthorModel
    {
        private readonly IAuthorRepository _repository;
        private readonly IDateValidator _dateValidator;

        public AuthorModel(IAuthorRepository repository, IDateValidator dateValidator)
        {
            _repository = repository;
            _dateValidator = dateValidator;
        }

        public async Task<int> CreateAuthorAsync(AuthorProxy author)
        {
            Validate(author);

            var entity = MapToEntity(author);

            _repository.Add(entity);

            await _repository.SaveChangesAsync();

            return entity.Id;
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

        private void Validate(AuthorProxy author)
        {
            // validate author

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