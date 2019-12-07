﻿using System.IO;
using Library.Filters;
using Library.Objects.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        private string ConnectionString => _configuration.GetConnectionString("Library");

        private string LoggingFolder => _configuration.GetValue<string>("LoggingFolder");

        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore()
                .AddJsonFormatters()
                .AddMvcOptions(AddMvcOptions);

            services.AddDbContext<LibraryContext>(options => options.UseSqlServer(ConnectionString));

            var logFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, LoggingFolder);

            ApplicationServices.Register(services, logFilePath);
        }

        public void Configure(IApplicationBuilder app)
        {
            SetupDatabase(app);
            app.UseMvc();
        }

        private void AddMvcOptions(MvcOptions options)
        {
            options.Filters.Add<AuthorizationFilter>();
            options.Filters.Add<ExceptionFilter>();
        }

        private void SetupDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<LibraryContext>();

                context.Database.EnsureCreated();
            }
        }
    }
}
