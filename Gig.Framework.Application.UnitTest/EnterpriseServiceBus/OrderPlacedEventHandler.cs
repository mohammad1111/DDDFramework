using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace Gig.Framework.Application.UnitTest.EnterpriseServiceBus
{
    //public class OrderPlacedEventHandler :
    //    IHandleMessages<OrderPlaced>
    //{
    //    public Task Handle(OrderPlaced message, IMessageHandlerContext context)
    //    {
    //        context.Publish(new OrderShipped() {OrderId = message.OrderId}).ConfigureAwait(false);
    //        return Task.CompletedTask;
    //    }
    //}
}