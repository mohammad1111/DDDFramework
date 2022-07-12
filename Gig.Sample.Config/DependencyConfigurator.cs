using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Gig.Framework.Core;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.DependencyInjection;
using Gig.Framework.ReadModel;
using Gig.Sample.Read.ReadModels;
using Gig.Sample.Write.Infrastructures.Persistence.Context;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Gig.Sample.Config
{
    public class DependencyConfigurator : IConfig
    {
        public void Config(IWindsorContainer builder)
        {
            //builder.Register(Component.For<IUnitOfWork>()
            //    .UsingFactoryMethod(() =>
            //        new TestDbContext(ServiceLocator.Setting.WriteDataConnectionString)).LifestyleScoped());

            ////builder.Register(Component.For<ReadDbContext>()
            //// .UsingFactoryMethod(() =>
            ////     new WpapDevContext(ServiceLocator.Setting.WriteDataConnectionString)).LifestyleScoped());

            builder.Register(Component.For<ReadDbContext>().ImplementedBy(typeof(WpapDevContext)).LifestyleTransient());


            builder.Register(Component.For<IUnitOfWork>()
                .UsingFactoryMethod(() =>
                {
                    IConfigurationRoot configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory() + "/App_Data/")
                        .AddJsonFile("dataSettings.json")
                        .Build();
                    var cnn = configuration.GetSection("WriteDataConnectionString").Value;
                    return new TestDbContext(cnn);
                })
                .LifestyleScoped());
        
        }

    }
}
