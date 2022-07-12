using Gig.Framework.Core;
using Gig.Framework.Core.Settings;

namespace Gig.Framework.ReadModel;

public abstract class ReadDbContextDependencies : IReadDbContextDependencies
{
    protected ReadDbContextDependencies(IDataSetting dataSetting, IRequestContext requestContext)
    {
        DataSetting = dataSetting;
        RequestContext = requestContext;
    }

    public IDataSetting DataSetting { get; }
    public IRequestContext RequestContext { get; }
}