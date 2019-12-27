using Library.Objects.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Library.OnStartup
{
    public static class Database
    {
        public static void Initialize(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<LibraryContext>();

                if (context.Database.EnsureCreated())
                {
                    context.Authors.AddRange(SeedData.Get());
                    context.SaveChanges();
                }
            }
        }
    }
}
