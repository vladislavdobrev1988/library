using Library.Objects.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Objects.Context
{
    public partial class LibraryContext
    {
        private void ConfigureSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();

            modelBuilder.Entity<AccessToken>().HasIndex(x => x.Token);
            modelBuilder.Entity<AccessToken>().HasIndex(x => x.Expires);

            SetDeleteBehavior(modelBuilder);
        }

        private void SetDeleteBehavior(ModelBuilder modelBuilder)
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
