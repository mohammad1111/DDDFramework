using Gig.Framework.Core.Settings;

namespace Gig.Framework.Core.DependencyInjection;

public interface IServiceLocator
{
    IGigContainer Current { get; }

    public IDataSetting Setting { get; }
    void Initial(IGigContainer container);
}