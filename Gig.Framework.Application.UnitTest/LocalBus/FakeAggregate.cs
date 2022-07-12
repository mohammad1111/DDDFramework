using Gig.Framework.Domain;

namespace Gig.Framework.Application.UnitTest.LocalBus
{
    public class FakeAggregate : AggregateRoot
    {
        public string Name { get; private set; }
        public FakeAggregate(string name)
        {
            Name = name;
            Changes.Add(new FakeAggregateCreatedEvent(Name));
        }
    }
}