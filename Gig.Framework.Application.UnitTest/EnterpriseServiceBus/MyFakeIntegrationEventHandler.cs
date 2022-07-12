using System.Threading.Tasks;
using MassTransit;
using NServiceBus;

namespace Gig.Framework.Application.UnitTest.EnterpriseServiceBus
{
    public class MyFakeIntegrationEventHandler :
        IConsumer<FakeIntegrationEvent>
    {
        //public Task Handle(FakeIntegrationEvent message, IMessageHandlerContext context)
        //{
        //    return Task.CompletedTask;
        //}

        public Task Consume(ConsumeContext<FakeIntegrationEvent> context)
        {
            return Task.CompletedTask;
        }
    }
}