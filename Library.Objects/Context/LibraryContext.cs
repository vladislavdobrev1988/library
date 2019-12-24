using Library.Objects.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Objects.Context
{
    public partial class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureEntityUser(modelBuilder);
            ConfigureEntityAuthor(modelBuilder);
            ConfigureEntityBook(modelBuilder);

            ConfigureDeleteBehavior(modelBuilder);
        }

#if DEBUG
        public override void Dispose()
        {
            base.Dispose();
        }
#endif
    }
}
