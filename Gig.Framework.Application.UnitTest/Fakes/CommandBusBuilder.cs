using Gig.Framework.Config;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Logging;

namespace Gig.Framework.Application.UnitTest.Fakes
{
    public class CommandBusBuilder
    {
        private ILogger _logger;
        private ICommandHandlerFactory _commandHandlerFactory;
        private IEventBus _eventBus;

        public CommandBusBuilder WithLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        public CommandBusBuilder WithCommandHandlerFactory(ICommandHandlerFactory commandHandlerFactory)
        {
            _commandHandlerFactory = commandHandlerFactory;
            return this;
        }

        public CommandBusBuilder WithEventBus(IEventBus eventBus)
        {
            _eventBus = eventBus;
            return this;
        }

        public ICommandBus Build()
        {
            _eventBus ??= new FakeEventBus(null);

            _commandHandlerFactory ??= new CommandHandlerFactory();
            _logger ??= new FakeLogger();



            return new Application.CommandBus(_commandHandlerFactory, _eventBus, _logger);
        }

    }
}