using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Gig.Sample.UI.Config
{
    public class ManagementSlContainerInstaller : IWindsorInstaller
    {
        #region IWindsorInstaller Members

        public void Install(IWindsorContainer container,
            IConfigurationStore store)
        {
            DependencyConfigurators.Config(container);
        }

        #endregion
    }
}