using Autofac;
using Gig.Framework.Core;
using Microsoft.AspNetCore.SignalR;

namespace Gig.Framework.Api.RealTimeServices;

public class DependencyConfigurator : IConfig
{
    public void Config(
        ContainerBuilder builder)
    {
        builder.RegisterType<GigNameUserIdProvider>().As<IUserIdProvider>().InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(RealtimeServiceDependencies<>)).As(typeof(IRealtimeServiceDependencies<>))
            .InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(RealtimeServiceDependencies<,>)).As(typeof(IRealtimeServiceDependencies<,>))
            .InstancePerLifetimeScope();
    }
}