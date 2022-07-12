using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using FluentAssertions;
using Gig.Framework.Application.UnitTest.Fakes;
using Gig.Framework.Bus;
using Gig.Framework.Config;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.DependencyInjection;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Models;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NServiceBus;
using ILogger = Gig.Framework.Core.Logging.ILogger;

namespace Gig.Framework.Application.UnitTest.CommandBus
{
    [TestClass]
    public class CommandBusTest
    {
        private WindsorContainer container;
        public CommandBusTest()
        {
            container = new WindsorContainer();
            ConfigDependency();
        }

        [TestMethod]
        public async Task When_Dispatch_A_Command_Then_CommandHandler_Handle_Method_Invoke()
        {
            var command = new FakeCommand();
            var commandBus = container.Resolve<ICommandBus>();
            var response = await commandBus.DispatchAsync<FakeCommand, FakeCommandResult>(command);
            response.Data.As<FakeCommandResult>().IsHandle.Should().BeTrue();
        }

        [TestMethod]
        public async Task When_Dispatch_A_Command_Then_Get_Expected_CommandResult()
        {
            var command = new FakeCommand();
            var commandBus = container.Resolve<ICommandBus>();
            var response = await commandBus.DispatchAsync<FakeCommand, FakeCommandResult>(command);
            response.Data.As<FakeCommandResult>().GetType().Should().Be(typeof(FakeCommandResult));
        }

        [TestMethod]
        public async Task When_Dispatch_A_Command_Without_Expected_CommandResult_Then_Get_GigCommonResultBase_As_Result()
        {
            var command = new FakeCommand();
            var commandBus = container.Resolve<ICommandBus>();
            var response = await commandBus.DispatchAsync(command);
            response.GetType().Should().Be(typeof(GigCommonResultBase));
        }

        protected virtual void ConfigDependency()
        {

            var currentAssembly = typeof(CommandBusTest).Assembly;
            var assemblies = currentAssembly
                .GetReferencedAssemblies()
                .Select(Assembly.Load).ToArray();
            container.CreateMassTransitBus(new BusConfig(Configurations.EndPointName)
            {
                ExternalContainer = container,
                BrokerUrl = "rabbitmq://172.31.0.156",
                BrokerUser = "admin",
                BrokerPassword = "admin"
            }, assemblies);


            container.Register(Component.For<ICommandBus>().ImplementedBy<Application.CommandBus>().LifestylePerThread());
            container.Register(Component.For<ICommandHandlerFactory>().ImplementedBy<CommandHandlerFactory>());
            container.Register(Component.For<ILogger>().ImplementedBy<FakeLogger>());
            container.Register(Component.For<IEnterpriseServiceBus>().ImplementedBy<GigEnterpriseServiceBus>());
            container.Register(Component.For<IEventBus>().ImplementedBy<FakeEventBus>());
            container.Register(Component.For<IUnitOfWork>().ImplementedBy<FakeUnitOfWork>());
            container.Register(Component.For<ICommandHandlerAsync<FakeCommand>>().ImplementedBy<FakeCommandHandler>());
            container.Register(Component.For<ICommandHandlerAsync<FakeCommand, FakeCommandResult>>().ImplementedBy<FakeCommandHandlerWithResult>());
            container.Register(Component.For<IGigContainer>().ImplementedBy<WindsorGigContainer>().LifestylePerThread());

            var serviceLocator = new ServiceLocator(new WindsorGigContainer(container));
            //  ServiceLocator.Current.CreateScope();
        }
    }
}
