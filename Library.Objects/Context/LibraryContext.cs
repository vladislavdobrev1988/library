using Library.Objects.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Objects.Context
{
    public partial class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureSchema(modelBuilder);
        }

#if DEBUG
        public override void Dispose()
        {
            base.Dispose();
        }
#endif
    }
}
