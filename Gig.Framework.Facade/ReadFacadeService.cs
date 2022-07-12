using Gig.Framework.Application.ReadModel;
using Gig.Framework.Core.FacadeServices;

namespace Gig.Framework.Facade;

public abstract class ReadFacadeService : IFacadeService
{
    protected readonly IQueryBus QueryBus;

    protected ReadFacadeService(IQueryBus queryBus)
    {
        QueryBus = queryBus;
    }
}