using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Gig.Framework.Application.UnitTest.Fakes;
using Gig.Framework.Bus;
using Gig.Framework.Config;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.DependencyInjection;
using Gig.Framework.Core.Events;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NServiceBus;
using Shared;

namespace Gig.Framework.Application.UnitTest.EnterpriseServiceBus
{
    [TestClass]
    public class EnterpriseServiceBusTest
    {
        private WindsorContainer container;
        public EnterpriseServiceBusTest()
        {
            container = new WindsorContainer();
            ConfigDependency();

        }
        [TestMethod]
        public async Task When_Publish_An_Event_That_Is_IExternalMessage_Then_Event_Must_Exist_In_OutBoxMessages()
        {

           // var bus = container.Resolve<IEnterpriseServiceBus>();// as FakeEventBus;
            var bus = container.Resolve<IBusControl>();
            await bus.StartAsync();
           // var integrationEvent = new FakeIntegrationEvent("FakeIntegrationEvent content") { };
          //  await bus.Publish(new OrderPlaced { OrderId = Guid.NewGuid() });
          await bus.Publish(new FakeIntegrationEvent("FakeIntegrationEvent content"));
            await Task.Delay(5000);

        }

        private void ConfigDependency()
        {
            container.Register(Component.For<ICommandBus>().ImplementedBy<Application.CommandBus>().LifestylePerThread());
            container.Register(Component.For<ICommandHandlerFactory>().ImplementedBy<CommandHandlerFactory>());
            var currentAssembly = typeof(EnterpriseServiceBusTest).Assembly;
            var assemblies = currentAssembly
                .GetReferencedAssemblies()
                .Select(Assembly.Load).ToArray();

            container.CreateMassTransitBus(new BusConfig(Configurations.EndPointName)
            {
                ExternalContainer = container,
                BrokerUrl = "rabbitmq://172.31.0.156",
                BrokerUser = "admin",
                BrokerPassword = "admin"
            },  assemblies);

            container.Register(Component.For<IEnterpriseServiceBus>().ImplementedBy<GigEnterpriseServiceBus>());
            container.Register(Component.For<IEventBus>().ImplementedBy<FakeEventBus>());
            container.Register(Component.For<IUnitOfWork>().ImplementedBy<FakeUnitOfWork>());
            container.Register(Component.For<ICommandHandlerAsync<FakeCommand>>().ImplementedBy<FakeCommandHandler>());
            //container.Register(Component.For<ICommandHandlerAsync<FakeCommand, FakeCommandResult>>().ImplementedBy<FakeCommandHandlerWithResult>());
            container.Register(Component.For<IGigContainer>().ImplementedBy<WindsorGigContainer>().LifestyleScoped());
            // container.Register(Component.For<IEventHandler<FakeEvent>>().ImplementedBy<MyFakeEventHandler>().LifestyleScoped());
            container.Register(Classes.FromAssemblyContaining(typeof(MyFakeIntegrationEventHandler))
                .Where(x => x.Name.EndsWith("EventHandler")).WithServiceAllInterfaces().LifestyleScoped());
            var serviceLocator = new ServiceLocator(new WindsorGigContainer(container));
            ServiceLocator.Current.CreateScope();
        }
    }
}