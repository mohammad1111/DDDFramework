using System.Threading.Tasks;
using Gig.Framework.Core.Events;

namespace Gig.Framework.Application.UnitTest.LocalBus
{
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
}