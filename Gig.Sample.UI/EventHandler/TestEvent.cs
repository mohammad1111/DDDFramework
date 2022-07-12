using System;

namespace Gig.Sample.UI.EventHandler
{
    public class TestEvent: ITestEvent
    {
        public Guid CorrelationEventId { get; set; }
    }
}