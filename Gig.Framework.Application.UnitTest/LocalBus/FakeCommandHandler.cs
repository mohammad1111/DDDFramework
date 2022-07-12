using System.Threading.Tasks;
using Gig.Framework.Application.UnitTest.Fakes;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.Events;

namespace Gig.Framework.Application.UnitTest.LocalBus
{
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
}