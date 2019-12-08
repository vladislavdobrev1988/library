using Library.Objects.Entities;
using Library.Objects.Exceptions;
using Library.Objects.Models.Interfaces;
using Library.Objects.Proxies;
using Library.Objects.Repositories.Interfaces;
using Library.Objects.Validation;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Threading.Tasks;

namespace Library.Objects.Models.Implementations
{
    public class UserModel : IUserModel
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher<User> _passwordHasher;

        private static class ErrorMessage
        {
            public const string USER_REQUIRED = "User is required";
            public const string EMAIL_EXISTS = "User with the same email already exists";
        }

        public UserModel(IUserRepository repository, IPasswordHasher<User> passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task SaveAsync(UserProxy user)
        {
            ValidateUser(user);

            var existing = await _repository.GetByEmail(user.Email);
            if (existing != null)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict, ErrorMessage.EMAIL_EXISTS);
            }

            var entity = MapToEntity(user);

            _repository.Add(entity);

            await _repository.SaveChangesAsync();
        }

        private User MapToEntity(UserProxy user)
        {
            return new User
            {
                Email = user.Email.ToLower(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                PasswordHash = _passwordHasher.HashPassword(null, user.Password)
            };
        }

        private void ValidateUser(UserProxy user)
        {
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, ErrorMessage.USER_REQUIRED);
            }

            var passwordError = Password.Validate(user.Password);
            if (passwordError != null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, passwordError);
            }
        }
    }
}
