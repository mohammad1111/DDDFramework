using Gig.Framework.Core;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.DependencyInjection;

namespace Gig.Framework.Persistence.Ef;

public class UnitOfWorkProvider : IUnitOfWorkProvider
{
    private readonly IServiceLocator _serviceLocator;

    public UnitOfWorkProvider(IServiceLocator serviceLocator)
    {
        _serviceLocator = serviceLocator;
    }

    public IUnitOfWork GetUnitOfWork()
    {
        return _serviceLocator.Current.Resolve<IUnitOfWork>();
    }

    public IRequestContext RequestContext()
    {
        return _serviceLocator.Current.Resolve<IRequestContext>();
    }
}