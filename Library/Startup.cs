using System.IO;
using Library.Filters;
using Library.Objects.Context;
using Library.OnStartup;
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

        private string LogFolder => _configuration.GetValue<string>("LogFolder");

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

            var logFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, LogFolder);

            ApplicationServices.Register(services, logFilePath);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
            Database.Setup(app);
        }

        private void AddMvcOptions(MvcOptions options)
        {
            options.Filters.Add<AuthorizationFilter>();
            options.Filters.Add<ExceptionFilter>();
        }
    }
}
