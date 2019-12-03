using Library.Objects.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Objects.Context
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

#if DEBUG
        public override void Dispose()
        {
            base.Dispose();
        }
#endif
    }
}
