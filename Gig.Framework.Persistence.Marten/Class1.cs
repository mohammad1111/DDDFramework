using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.Events;
using Marten;

namespace Gig.Framework.Persistence.Marten;

public class MartinUnitOfWork :IUnitOfWork
{
    private readonly IDocumentStore _store;

    public MartinUnitOfWork(IDocumentStore store)
    {
        _store = store;
        var s=_store.OpenSession();
        
    }
    public Task BeginTransaction()
    {
        var s=_store.OpenSession();
    }

    public Task<IEnumerable<PublisherEvent>> Commit(IList<GigEntityRulesResult> ruleSetIds = null)
    {
        throw new NotImplementedException();
    }

    public void Rollback()
    {
        throw new NotImplementedException();
    }
}