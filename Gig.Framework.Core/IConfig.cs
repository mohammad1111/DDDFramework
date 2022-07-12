using Autofac;

namespace Gig.Framework.Core;

public interface IConfig
{
    void Config(ContainerBuilder builder);
}