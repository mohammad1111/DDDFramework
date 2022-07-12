using Gig.Framework.Core.Caching;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.DependencyInjection;
using Gig.Framework.Core.Serilizer;
using Gig.Framework.Core.Settings;
using Gig.Framework.Persistence.Ef;
using Gig.Sample.Write.Domains.Domain.User;
using Gig.Sample.Write.Domains.Domain.Product;
using Microsoft.EntityFrameworkCore;

namespace Gig.Sample.Write.Infrastructures.Persistence.Context
{
    public class TestDbContext : EfUnitOfWork
    {
        public TestDbContext(string connectionString)
            : base(connectionString, ServiceLocator.Current.Resolve<IEntityFrameWorkDependencies>())
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Product> Products { get; set; }
    }
}
