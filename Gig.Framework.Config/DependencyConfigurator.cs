using Autofac;
using Gig.Framework.Application;
using Gig.Framework.Application.EventScheduling;
using Gig.Framework.Application.ReadModel;
using Gig.Framework.Bus;
using Gig.Framework.Bus.GigRoutingSlip;
using Gig.Framework.Bus.GigStateMachine;
using Gig.Framework.Bus.RequestClient;
using Gig.Framework.Caching;
using Gig.Framework.Core;
using Gig.Framework.Core.Caching;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.DataProviders.Convertors;
using Gig.Framework.Core.DataProviders.Elastic;
using Gig.Framework.Core.DependencyInjection;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Helper;
using Gig.Framework.Core.RuleEngine;
using Gig.Framework.Core.Security;
using Gig.Framework.Core.Serilizer;
using Gig.Framework.Core.Settings;
using Gig.Framework.Core.UserContexts;
using Gig.Framework.Data.Elastic;
using Gig.Framework.DependencyInjection.Autofac;
using Gig.Framework.Domain.IdGenerators;
using Gig.Framework.Persistence.Ef;
using Gig.Framework.Persistence.Ef.Converts;
using Gig.Framework.RuleEngine;
using Gig.Framework.RuleEngine.Contract.Contracts;
using Gig.Framework.Scheduling;
using Gig.Framework.Workflow;
using Microsoft.Extensions.Configuration;

namespace Gig.Framework.Config;

public class DependencyConfigurator
{
    public static void Config(ContainerBuilder builder)
    {
        builder.RegisterType<GigScheduler>().As<IGigScheduler>().SingleInstance();
        builder.RegisterType<EventAggregator>().As<IEventBus>().InstancePerLifetimeScope();
        builder.RegisterType<RuleRepository>().As<IRuleRepository>().SingleInstance();
        builder.RegisterType<CrcEngine>().As<ICrcEngine>().SingleInstance();
        builder.RegisterType<EventRepository>().As<IEventRepository>().InstancePerLifetimeScope();
        builder.RegisterType<CrudAuditLogElasticProvider>().As<ICrudAuditLogElasticProvider>().InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(ElasticProvider<>)).As(typeof(IElasticProvider<>)).InstancePerLifetimeScope();
        builder.RegisterType<AutoFacGigContainer>().As<IGigContainer>().InstancePerLifetimeScope();
        builder.RegisterType<GigMapper>().As<IMapper>().SingleInstance();
        builder.RegisterType<GigEnterpriseServiceBus>().As<IEnterpriseServiceBus>().InstancePerLifetimeScope();
        builder.Register(_ => new JsonSerializer(new PointConvert(), new LongStringConverter(), new NullableLongStringConverter())).As<ISerializer>().SingleInstance();
        builder.RegisterType<QueryBus>().As<IQueryBus>().InstancePerLifetimeScope();
        builder.RegisterType<MemoryCaching>().As<IMemoryCacheManager>().SingleInstance();
        builder.RegisterType<RequestMemoryCaching>().As<IRequestMemoryCacheManager>().InstancePerLifetimeScope();
        builder.RegisterType<RedisCache>().As<IDistributeCacheManager>().SingleInstance();
        builder.RegisterType<UserAccessDataProvider>().As<IUserAccessDataProvider>().SingleInstance();
        builder.RegisterType<EventPublisherJob>().AsSelf().InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(CqrsGigWorkflowEngine<,>)).As(typeof(ICqrsGigWorkflowEngine<,>)).InstancePerLifetimeScope();
        //  builder.RegisterGeneric(typeof(SimpleGigWorkflowEngine<,>)).As(typeof(ISimpleGigWorkflowEngine<,>)).InstancePerLifetimeScope();

        builder.RegisterType<HandleMessageDependencies>().As<IHandleMessageDependencies>().InstancePerLifetimeScope();
        builder.RegisterType<EntityFrameWorkDependencies>().As<IEntityFrameWorkDependencies>().InstancePerLifetimeScope();
        builder.RegisterType<GigRoutingSlipActivityDependencies>().As<IGigRoutingSlipActivityDependencies>().InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(RuleEngine<>)).As(typeof(IRuleEngine<>)).InstancePerLifetimeScope();
        builder.RegisterType<ServiceLocator>().As<IServiceLocator>().InstancePerLifetimeScope();
        builder.RegisterType<CommandBus>().As<ICommandBus>().InstancePerLifetimeScope();
        builder.RegisterType<CommandHandlerFactory>().As<ICommandHandlerFactory>().InstancePerLifetimeScope();
        builder.Register(c => new DataSettingReader(c.Resolve<IConfiguration>()).Read()).As<IDataSetting>().SingleInstance();
        builder.RegisterType<UserContextService>().As<IUserContextService>().InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(GigRequestClient<>)).As(typeof(IGigRequestClient<>)).SingleInstance();
        builder.RegisterType<SecurityManager>().As<ISecurityManager>().InstancePerLifetimeScope();
        builder.RegisterType<RequestContext>().As<IRequestContext>().InstancePerLifetimeScope();
        builder.RegisterType<UnitOfWorkProvider>().As<IUnitOfWorkProvider>().InstancePerLifetimeScope();
        builder.RegisterType<GigActivityDependencies>().As<IGigActivityDependencies>().InstancePerLifetimeScope();
        builder.Register(x => x.Resolve<GigDbContext>()).As<IUnitOfWork>().InstancePerLifetimeScope();

        builder.Register(x => new GigIdGenerator()).As<IGigIdGenerator>().SingleInstance();
    }
}