using System;
using System.Collections.Generic;
using Library.Objects.Entities;

namespace Library.Objects.Context
{
    public static class SeedData
    {
        public static Author[] Get()
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
                            PublishDate = DateTime.Parse("1939-04-14")
                        },
                        new Book
                        {
                            Title = "East of Eden",
                            PublishDate = DateTime.Parse("1952-09-19")
                        }
                    }
                },
                new Author
                {
                    FirstName = "Harper",
                    LastName = "Lee",
                    DateOfBirth = DateTime.Parse("1926-04-28"),
                    DateOfDeath = DateTime.Parse("2016-02-19"),
                    NobelPrize = false,
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Title = "To Kill a Mockingbird",
                            PublishDate = DateTime.Parse("1960-07-11")
                        }
                    }
                }
            };
        }
    }
}
