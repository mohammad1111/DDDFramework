using System.Threading.Tasks;
using Gig.Framework.Core.Events;

namespace Gig.Framework.Application.UnitTest.LocalBus
{
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
}