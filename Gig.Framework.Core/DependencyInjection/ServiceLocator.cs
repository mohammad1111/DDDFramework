using Gig.Framework.Core.Settings;

namespace Gig.Framework.Core.DependencyInjection;

public class ServiceLocator : IServiceLocator
{
    public ServiceLocator(IGigContainer container)
    {
        //Current = container;
        Current = container;
    }

    public IDataSetting Setting => Current.Resolve<IDataSetting>();

    public void Initial(IGigContainer container)
    {
        Current = container;
    }

    public IGigContainer Current { get; private set; }

    public IRequestContext UserContext()
    {
        return Current.Resolve<IRequestContext>();
    }
}