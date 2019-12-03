using Library.Helpers.Authorization;
using Library.Objects.Entities;
using Library.Objects.Models.Implementations;
using Library.Objects.Models.Interfaces;
using Library.Objects.Repositories.Implementations;
using Library.Objects.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Library
{
    public static class ApplicationServices
    {
        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IAccessTokenStore, AccessTokenStore>();
            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserModel, UserModel>();
        }
    }
}
