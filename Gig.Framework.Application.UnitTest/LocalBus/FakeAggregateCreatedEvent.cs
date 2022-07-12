using Gig.Framework.Core.Events;

namespace Gig.Framework.Application.UnitTest.LocalBus
{
    public class FakeAggregateCreatedEvent : DomainEvent
    {
        public string Name { get; private set; }
        public FakeAggregateCreatedEvent(string name)
        {
            Name = name;
        }
    }
}