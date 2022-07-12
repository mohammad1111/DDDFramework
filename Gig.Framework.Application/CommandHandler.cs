using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.Events;

namespace Gig.Framework.Application;

public abstract class CommandHandler
{
    public IList<IEvent> Events { get; } = new List<IEvent>();


    public void AddEvent(IEvent @event)
    {
        Events.Add(@event);
    }
    
    protected CommandHandler(IUnitOfWorkProvider unitOfWorkProvider)
    {
        UnitOfWorkWorkProvider = unitOfWorkProvider;
    }

    public IUnitOfWorkProvider UnitOfWorkWorkProvider { get; }
}