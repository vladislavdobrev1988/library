using Library.Objects.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Objects.Context
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureUser(modelBuilder);
        }

        private void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        }

#if DEBUG
        public override void Dispose()
        {
            base.Dispose();
        }
#endif
    }
}
