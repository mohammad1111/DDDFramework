using Gig.Framework.Core;
using Gig.Framework.Core.Caching;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.DependencyInjection;
using Gig.Framework.Core.RuleEngine;
using Gig.Framework.Core.Serilizer;
using Gig.Framework.Core.Settings;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Gig.Framework.Persistence.Ef;

public class EntityFrameWorkDependencies : IEntityFrameWorkDependencies
{
    public EntityFrameWorkDependencies(ISerializer serializer, IDataSetting dataSetting,
        IMemoryCacheManager memoryCacheManager, IDistributeCacheManager distributeCacheManager, ILogger logger,
        IRequestMemoryCacheManager requestMemoryCacheManager, IRuleRepository ruleRepository,
        IServiceLocator serviceLocator, IRequestContext requestContext
        )
    {
        Serializer = serializer;
        DataSetting = dataSetting;
        MemoryCacheManager = memoryCacheManager;
        DistributeCacheManager = distributeCacheManager;
        Logger = logger;
        RequestMemoryCacheManager = requestMemoryCacheManager;
        RuleRepository = ruleRepository;
        ServiceLocator = serviceLocator;
        RequestContext = requestContext;
    }


    public IServiceLocator ServiceLocator { get; }


    public ILogger Logger { get; }


    public ISerializer Serializer { get; }


    public IDataSetting DataSetting { get; }
    public IRequestContext RequestContext { get; }


    public IMemoryCacheManager MemoryCacheManager { get; }


    public IDistributeCacheManager DistributeCacheManager { get; }


    public IRequestMemoryCacheManager RequestMemoryCacheManager { get; }


    public IRuleRepository RuleRepository { get; }
}