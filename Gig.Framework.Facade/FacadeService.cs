using Gig.Framework.Application;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.FacadeServices;

namespace Gig.Framework.Facade;

public abstract class FacadeService : IFacadeService
{
    protected readonly ICommandBus CommandBus;
    protected readonly IEventBus EventBus;


    protected FacadeService(IEventBus eventBus, ICommandBus commandBus)
    {
        CommandBus = commandBus;
        EventBus = eventBus;
    }
}