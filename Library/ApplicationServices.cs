﻿using System;
using Library.Helpers.Authorization;
using Library.Objects.Entities;
using Library.Objects.Models.Implementations;
using Library.Objects.Models.Interfaces;
using Library.Objects.Repositories.Implementations;
using Library.Objects.Repositories.Interfaces;
using Library.Objects.Services.Implementations;
using Library.Objects.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Library
{
    public static class ApplicationServices
    {
        public static void Register(IServiceCollection services, string logFilePath)
        {
            services.AddSingleton<ITextAppender, FileTextAppender>(x => new FileTextAppender(logFilePath, GetLogFileName));
            services.AddSingleton<IExceptionLogger, ExceptionLogger>();
            services.AddSingleton<IAccessTokenStore, AccessTokenStore>();
            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserModel, UserModel>();
        }

        private static string GetLogFileName()
        {
            var date = DateTime.UtcNow.ToString("yyyy-dd");
            return string.Concat(date, ".txt");
        }
    }
}
