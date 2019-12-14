using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Models.Base;
using Library.Objects.Models.Interfaces;
using Library.Objects.Proxies;
using Library.Objects.Repositories.Interfaces;
using Library.Objects.Validation;
using Microsoft.AspNetCore.Identity;

namespace Library.Objects.Models.Implementations
{
    public class UserModel : BaseModel, IUserModel
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public static class ErrorMessage
        {
            public const string USER_REQUIRED = "User is required";
            public const string FIRST_NAME_REQUIRED = "First name is required";
            public const string LAST_NAME_REQUIRED = "Last name is required";
            public const string EMAIL_EXISTS = "User with the same email already exists";
        }

        public UserModel(IUserRepository repository, IPasswordHasher<User> passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task CreateUserAsync(UserProxy user)
        {
            ValidateUser(user);

            var existing = await _repository.GetByEmail(user.Email);
            if (existing != null)
            {
                ThrowHttpConflict(ErrorMessage.EMAIL_EXISTS);
            }

            var entity = MapToEntity(user);

            _repository.Add(entity);

            await _repository.SaveChangesAsync();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ThrowHttpBadRequest(Email.ErrorMessage.REQUIRED);
            }

            return await _repository.GetByEmail(email);
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
                ThrowHttpBadRequest(ErrorMessage.USER_REQUIRED);
            }

            var emailError = Email.Validate(user.Email);
            if (emailError != null)
            {
                ThrowHttpBadRequest(emailError);
            }

            var passwordError = Password.Validate(user.Password);
            if (passwordError != null)
            {
                ThrowHttpBadRequest(passwordError);
            }

            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                ThrowHttpBadRequest(ErrorMessage.FIRST_NAME_REQUIRED);
            }

            if (string.IsNullOrWhiteSpace(user.LastName))
            {
                ThrowHttpBadRequest(ErrorMessage.LAST_NAME_REQUIRED);
            }
        }
    }
}
