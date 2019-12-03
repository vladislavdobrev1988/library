﻿using Library.Filters;
using Library.Objects.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Library
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore()
                .AddJsonFormatters()
                .AddMvcOptions(AddMvcOptions);

            var connectionString = _configuration.GetConnectionString("Library");

            services.AddDbContext<LibraryContext>(options => options.UseSqlServer(connectionString));

            ApplicationServices.Register(services);            
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<LibraryContext>();

                context.Database.EnsureCreated();
            }
            
            app.UseMvc();
        }

        private void AddMvcOptions(MvcOptions options)
        {
            options.Filters.Add<AuthorizationFilter>();
            options.Filters.Add<HttpResponseExceptionFilter>();
        }
    }
}
