using Library.Objects.Entities;
using Library.Objects.Helpers.Constants;
using Microsoft.EntityFrameworkCore;

namespace Library.Objects.Context
{
    public partial class LibraryContext
    {
        private void ConfigureEntityUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        }

        private void ConfigureEntityAuthor(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasIndex(nameof(Author.FirstName), nameof(Author.LastName)).IsUnique();

            modelBuilder.Entity<Author>().Property(x => x.DateOfBirth).HasColumnType(SqlDataType.DATE);
            modelBuilder.Entity<Author>().Property(x => x.DateOfDeath).HasColumnType(SqlDataType.DATE);
        }

        private void ConfigureEntityBook(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().Property(x => x.ReleaseDate).HasColumnType(SqlDataType.DATE);
        }

        private void ConfigureDeleteBehavior(ModelBuilder modelBuilder)
        {
            foreach (var type in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var foreignKey in type.GetForeignKeys())
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
                }
            }
        }
    }
}
