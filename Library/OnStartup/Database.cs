using System;
using System.Collections.Generic;
using Library.Objects.Context;
using Library.Objects.Entities;
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
                    context.Authors.AddRange(GetSeedData());
                    context.SaveChanges();
                }
            }
        }

        private static Author[] GetSeedData()
        {
            return new Author[]
            {
                new Author
                {
                    FirstName = "John",
                    LastName = "Steinbeck",
                    DateOfBirth = DateTime.Parse("1902-02-27"),
                    DateOfDeath = DateTime.Parse("1968-12-20"),
                    NobelPrize = true,
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Title = "The Grapes of Wrath",
                            ReleaseDate = DateTime.Parse("1939-04-14")
                        },
                        new Book
                        {
                            Title = "East of Eden",
                            ReleaseDate = DateTime.Parse("1952-09-19")
                        }
                    }
                }
            };
        }
    }
}
