using Library.Objects.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Library.OnStartup
{
    public static class Database
    {
        public static void Setup(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<LibraryContext>();

                context.Database.EnsureCreated();
            }
        }
    }
}
