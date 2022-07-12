using Gig.Framework.ReadModel;
using Microsoft.EntityFrameworkCore;

namespace Gig.Sample.Read.ReadModels
{
    public class WpapDevContext : ReadDbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReadModels.User.User>(entity => { entity.ToTable("Users", "dbo"); });

            modelBuilder.Entity<ReadModels.Product.Product>(entity => { entity.ToTable("Products", "dbo"); });
        }
    }
}
