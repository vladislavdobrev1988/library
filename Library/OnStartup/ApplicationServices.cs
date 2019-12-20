using System.IdentityModel.Tokens.Jwt;
using Library.Objects.Entities;
using Library.Objects.Models.Implementations;
using Library.Objects.Models.Interfaces;
using Library.Objects.Repositories.Implementations;
using Library.Objects.Repositories.Interfaces;
using Library.Objects.Services.Implementations;
using Library.Objects.Services.Interfaces;
using Library.Objects.Validation.Implementations;
using Library.Objects.Validation.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Library.OnStartup
{
    public static class ApplicationServices
    {
        public static void Register(IServiceCollection services, string logFilePath)
        {
            services.AddSingleton<JwtSecurityTokenHandler>();

            services.AddSingleton<IExceptionLogger, ExceptionLogger>(x => new ExceptionLogger(logFilePath));
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<IAccessTokenManager, AccessTokenManager>();
            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddSingleton<IEmailValidator, EmailValidator>();
            services.AddSingleton<IPasswordValidator, PasswordValidator>();

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IAccountModel, AccountModel>();
            services.AddScoped<IUserModel, UserModel>();
        }
    }
}
