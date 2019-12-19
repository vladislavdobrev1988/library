using System.Threading.Tasks;
using Library.Objects.Entities;
using Library.Objects.Helpers.Common;
using Library.Objects.Helpers.Constants;
using Library.Objects.Models.Interfaces;
using Library.Objects.Proxies;
using Library.Objects.Repositories.Interfaces;
using Library.Objects.Validation.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Library.Objects.Models.Implementations
{
    public class UserModel : IUserModel
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEmailValidator _emailValidator;
        private readonly IPasswordValidator _passwordValidator;

        public static class ErrorMessage
        {
            public const string USER_REQUIRED = "User is required";
            public const string FIRST_NAME_REQUIRED = "First name is required";
            public const string LAST_NAME_REQUIRED = "Last name is required";
            public const string EMAIL_EXISTS = "User with the same email already exists";
        }

        public UserModel(IUserRepository repository, IPasswordHasher<User> passwordHasher, IEmailValidator emailValidator, IPasswordValidator passwordValidator)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _emailValidator = emailValidator;
            _passwordValidator = passwordValidator;
        }

        public async Task CreateUserAsync(UserProxy user)
        {
            ValidateUser(user);

            var existing = await _repository.GetByEmail(user.Email);
            if (existing != null)
            {
                ThrowHttp.Conflict(ErrorMessage.EMAIL_EXISTS);
            }

            var entity = MapToEntity(user);

            _repository.Add(entity);

            await _repository.SaveChangesAsync();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ThrowHttp.BadRequest(CommonErrorMessage.EMAIL_REQUIRED);
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
                ThrowHttp.BadRequest(ErrorMessage.USER_REQUIRED);
            }

            var emailError = _emailValidator.Validate(user.Email);
            if (emailError != null)
            {
                ThrowHttp.BadRequest(emailError);
            }

            var passwordError = _passwordValidator.Validate(user.Password);
            if (passwordError != null)
            {
                ThrowHttp.BadRequest(passwordError);
            }

            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                ThrowHttp.BadRequest(ErrorMessage.FIRST_NAME_REQUIRED);
            }

            if (string.IsNullOrWhiteSpace(user.LastName))
            {
                ThrowHttp.BadRequest(ErrorMessage.LAST_NAME_REQUIRED);
            }
        }
    }
}
