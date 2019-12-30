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
                },
                new Author
                {
                    FirstName = "Pelham",
                    LastName = "Wodehouse",
                    DateOfBirth = DateTime.Parse("1881-10-15"),
                    DateOfDeath = DateTime.Parse("1975-02-14"),
                    NobelPrize = false,
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Title = "Joy in the Morning",
                            PublishDate = DateTime.Parse("1946-08-22")
                        },
                        new Book
                        {
                            Title = "Psmith in the City",
                            PublishDate = DateTime.Parse("1910-09-23")
                        },
                        new Book
                        {
                            Title = "Uncle Dynamite",
                            PublishDate = DateTime.Parse("1948-10-22")
                        }
                    }
                },
                new Author
                {
                    FirstName = "Kurt",
                    LastName = "Vonnegut",
                    DateOfBirth = DateTime.Parse("1922-11-11"),
                    DateOfDeath = DateTime.Parse("2007-04-11"),
                    NobelPrize = false,
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Title = "Breakfast of Champions",
                            PublishDate = DateTime.Parse("1973-07-12")
                        },
                        new Book
                        {
                            Title = "Mother Night",
                            PublishDate = DateTime.Parse("1961-04-17")
                        }
                    }
                }
            };
        }
    }
}
