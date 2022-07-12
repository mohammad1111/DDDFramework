using System;
using Gig.Framework.Core.Events;

namespace Gig.Framework.Application.UnitTest.LocalBus
{
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
}