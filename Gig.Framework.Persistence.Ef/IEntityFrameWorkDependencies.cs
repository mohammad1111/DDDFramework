using Gig.Framework.Core;
using Gig.Framework.Core.Caching;
using Gig.Framework.Core.DependencyInjection;
using Gig.Framework.Core.RuleEngine;
using Gig.Framework.Core.Serilizer;
using Gig.Framework.Core.Settings;
using Serilog;

namespace Gig.Framework.Persistence.Ef;

public interface IEntityFrameWorkDependencies
{


    IServiceLocator ServiceLocator { get; }

    ILogger Logger { get; }

    ISerializer Serializer { get; }


    IDataSetting DataSetting { get; }

    IRequestContext RequestContext { get; }

    IMemoryCacheManager MemoryCacheManager { get; }

    IDistributeCacheManager DistributeCacheManager { get; }

    IRequestMemoryCacheManager RequestMemoryCacheManager { get; }


    IRuleRepository RuleRepository { get; }
    
    
}