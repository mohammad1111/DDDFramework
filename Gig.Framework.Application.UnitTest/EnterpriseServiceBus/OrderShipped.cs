using System;
using Gig.Framework.Core.Events;
using NServiceBus;

namespace Shared
{
    public class OrderShipped : IIntegrationMessage
    {
        public Guid OrderId { get; set; }

    }

    public class OrderBilled : IIntegrationMessage
    {
        public Guid OrderId { get; set; }

    }

    public class OrderPlaced :
        IIntegrationMessage
    {
        public Guid OrderId { get; set; }
    }

    public class CompleteOrder :
        IMessage
    {
        public Guid OrderId { get; set; }
    }

    public class OrderQuantityChecked : IIntegrationMessage
    {
        public Guid OrderId { get; set; }

    }


   

}
public class OrderSagaData :
    IContainSagaData
{
    public Guid Id { get; set; }
    public string Originator { get; set; }
    public string OriginalMessageId { get; set; }
    public Guid OrderId { get; set; }
    public bool IsComplete { get; set; }
    public bool IsShipped { get; set; }

    public bool IsPlaced { get; set; }
    public bool IsCanceled { get; set; }
    public bool IsBilled { get; set; }
    public bool IsQuantityChecked { get; set; }
}