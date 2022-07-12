using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using FluentAssertions;
using Gig.Framework.Application.UnitTest.CommandBus;
using Gig.Framework.Application.UnitTest.Fakes;
using Gig.Framework.Bus;
using Gig.Framework.Config;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.DependencyInjection;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Logging;
using Gig.Framework.Domain;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NServiceBus;
using IEvent = Gig.Framework.Core.Events.IEvent;

namespace Gig.Framework.Application.UnitTest
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
        public async Task When_Publish_An_Event_Then_EventHandler_Must_Called()
        {
            var bus = container.Resolve<IEventBus>() as FakeEventBus;
            bool fakeEventHandeld = false;
            int handleCount = 0;
            bus.Subscribe(new ActionHandler<FakeEvent>(a =>
            {
                handleCount += 1;
            }));

            bus.Subscribe<FakeEvent>((a) =>
            {
                handleCount += 1;
            });

            IEvent fakeEvent = new FakeEvent("Test local Event");
            await bus.PublishAsync(fakeEvent);

            fakeEventHandeld.Should().BeTrue();
        }

        [TestMethod]
        public async Task When_Dispatch_A_Command_Then_The_Published_DomainEvent_Must_Handled()
        {
            var commandBus = container.Resolve<ICommandBus>();
            var eventBus = container.Resolve<IEventBus>();
            bool fakeEventHandeld = false;
            await eventBus.PublishAsync(new FakeEvent("Test local Event") { });
            await commandBus.DispatchAsync(new FakeCommand());

            

            fakeEventHandeld.Should().BeTrue();
        }

        [TestMethod]
        public async Task When_Publish_An_Event_That_Is_IIntegrationMessage_Then_Event_Must_Exist_In_OutBoxMessages()
        {
            var bus = container.Resolve<IEventBus>() as FakeEventBus;

            var integrationEvent = new FakeIntegrationEvent("Integration Event") { };
            await bus.PublishAsync(integrationEvent);
            var outBoxMessage = bus.OutBoxMessages.FirstOrDefault() as FakeIntegrationEvent;
            integrationEvent.Id.Should().Equals(outBoxMessage.Id);
        }

        [TestMethod]
        public async Task When_Publish_An_Event_That_Is_IExternalMessage_Then_Event_Must_Exist_In_OutBoxMessages()
        {

            var bus = container.Resolve<IEnterpriseServiceBus>();// as FakeEventBus;

            var integrationEvent = new FakeIntegrationEvent("Integration 555") { };
            await bus.Publish(integrationEvent);
            //await bus.PublishAsync(new TransactionCommitedEvent());

            while (true)
            {

            }
        }


        private void ConfigDependency()
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
            //container.Register(Component.For<ICommandHandlerAsync<FakeCommand, FakeCommandResult>>().ImplementedBy<FakeCommandHandlerWithResult>());
            container.Register(Component.For<IGigContainer>().ImplementedBy<WindsorGigContainer>().LifestyleScoped());
            // container.Register(Component.For<IEventHandler<FakeEvent>>().ImplementedBy<MyFakeEventHandler>().LifestyleScoped());
            container.Register(Classes.FromAssemblyContaining(typeof(MyFakeIntegrationEventHandler))
                .Where(x => x.Name.EndsWith("EventHandler")).WithServiceAllInterfaces().LifestyleScoped());
            var serviceLocator = new ServiceLocator(new WindsorGigContainer(container));
            ServiceLocator.Current.CreateScope();
        }
    }

    public class MyFakeIntegrationEventHandler :
        IConsumer<FakeIntegrationEvent>
    {
        public Task Consume(ConsumeContext<FakeIntegrationEvent> context)
        {
            return Task.CompletedTask;
        }
    }

    public class MyFakeEventHandler :
        IEventHandler<FakeEvent>
    {
        private readonly IEventBus _eventBus;
        public MyFakeEventHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        public async Task HandleAsync(FakeEvent eventToHandle)
        {
            await _eventBus.PublishAsync(new FakeEventHandledEvent());
        }
    }

    public class FakeCommandHandler : CommandHandler, ICommandHandlerAsync<FakeCommand>
    {
        private readonly IEventBus _bus;
        public FakeCommandHandler(IUnitOfWork uow, IEventBus bus) : base(uow)
        {
            _bus = bus;
        }

        public Task HandleAsync(FakeCommand command)
        {

            _bus.PublishAsync(new FakeAggregateCreatedEvent("myAggregate "));
            return Task.CompletedTask;
        }
    }
    public class FakeAggregateCreatedEventHandler :
        IEventHandler<FakeAggregateCreatedEvent>
    {

        public FakeAggregateCreatedEventHandler()
        {

        }
        public Task HandleAsync(FakeAggregateCreatedEvent eventToHandle)
        {
            return Task.CompletedTask;
        }
    }

    public class FakeAggregate : AggregateRoot
    {
        public string Name { get; private set; }
        public FakeAggregate(string name)
        {
            Name = name;
            Changes.Add(new FakeAggregateCreatedEvent(Name));
        }
    }

    public class FakeAggregateCreatedEvent : DomainEvent
    {
        public string Name { get; private set; }
        public FakeAggregateCreatedEvent(string name)
        {
            Name = name;
        }
    }


    public class FakeEvent : DomainEvent
    {
        public DateTime CreatedTime { get; private set; }
        public string Content { get; private set; }
        public FakeEvent(string content)
        {
            CreatedTime = DateTime.Now;
            Content = content;
        }
    }

    public class FakeEventHandledEvent : IEvent
    {
    }

    public class FakeIntegrationEvent : DomainEvent, IIntegrationMessage
    {
        public Guid Id { get; private set; }
        public DateTime CreatedTime { get; private set; }
        public string Content { get; private set; }
        public FakeIntegrationEvent(string content)
        {
            Id = Guid.NewGuid();
            CreatedTime = DateTime.Now;
            Content = content;
        }
    }
}
