using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.Events;
using Microsoft.EntityFrameworkCore;

namespace Gig.Framework.Persistence.Ef;

public class GigDbContext:DbContext,IUnitOfWork
{
    public GigDbContext(DbContextOptions<EfUnitOfWork> options):base(options)
    {
        
    }
    public virtual Task BeginTransaction()
    {
       return Task.CompletedTask;
    }

    public virtual Task<IEnumerable<PublisherEvent>> Commit(IList<GigEntityRulesResult> ruleSetIds = null)
    {
        throw new NotImplementedException();
    }

    public virtual void Rollback()
    {
    }

    public DbContext GetDbContext()
    {
        return this;
    }
}