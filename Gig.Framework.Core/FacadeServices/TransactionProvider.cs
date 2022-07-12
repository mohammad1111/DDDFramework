using System.Transactions;
using Gig.Framework.Core.Caching;
using Gig.Framework.Core.Events;

namespace Gig.Framework.Core.FacadeServices;

public static class TransactionProvider
{
    private static TransactionScope CreateTransactionScope()
    {
        var txOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadUncommitted
        };
        return new TransactionScope(TransactionScopeOption.Required, txOptions,
            TransactionScopeAsyncFlowOption.Enabled);
    }

    public static async Task RunInTransaction(Func<Task> action, CancellationToken token,
        IRequestMemoryCacheManager requestMemoryCacheManager, IEventBus eventBus)
    {
        using var scope = CreateTransactionScope();
        var transactionId = Guid.NewGuid();
        await requestMemoryCacheManager.AddAsync(new TransactionIdCacheKey(), transactionId);
        await action.Invoke();
        if (token.IsCancellationRequested)
        {
            scope.Dispose();
            return;
        }

        scope.Complete();
        scope.Dispose();

        await eventBus.PublishLocalMessageAsync(new TransactionCommitEvent
        {
            TransactionEventId = transactionId
        });
        
        
        
    }
}