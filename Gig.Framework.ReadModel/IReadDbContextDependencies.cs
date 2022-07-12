using Gig.Framework.Core;
using Gig.Framework.Core.Settings;

namespace Gig.Framework.ReadModel;

public interface IReadDbContextDependencies
{
    IDataSetting DataSetting { get; }
    IRequestContext RequestContext { get; }
}