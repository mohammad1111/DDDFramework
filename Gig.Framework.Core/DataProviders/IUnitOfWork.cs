using Gig.Framework.Core.Events;

namespace Gig.Framework.Core.DataProviders;

public interface IUnitOfWork
{
    Task BeginTransaction();
    Task<IEnumerable<PublisherEvent>> Commit(IList<GigEntityRulesResult> ruleSetIds = null);
    void Rollback();
}