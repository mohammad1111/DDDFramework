using Autofac;
using Gig.Framework.Api.Controllers;
using Gig.Framework.Core;

namespace Gig.Framework.Api;

public class DependencyConfigurator : IConfig
{
    public void Config(ContainerBuilder builder)
    {
        builder.RegisterType<GigControllerDependencies>().As<IGigControllerDependencies>()
            .InstancePerLifetimeScope();
        builder.RegisterType<GigControllerDependencies>().As<IGigControllerDependencies>().InstancePerLifetimeScope();
    }
}