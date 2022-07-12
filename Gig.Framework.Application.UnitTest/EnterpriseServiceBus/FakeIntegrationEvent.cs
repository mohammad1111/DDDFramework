using System;

namespace Gig.Framework.Application.UnitTest.EnterpriseServiceBus
{
    public class FakeIntegrationEvent : Core.Events.IIntegrationMessage
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