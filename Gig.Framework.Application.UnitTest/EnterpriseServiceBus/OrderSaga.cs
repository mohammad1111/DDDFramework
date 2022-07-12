using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shared;

public class OrderSaga :
    Saga<OrderSagaData>,
    IAmStartedByMessages<OrderPlaced>
    //IAmStartedByMessages<OrderBilled>,
    //IAmStartedByMessages<OrderQuantityChecked>,
    //IAmStartedByMessages<OrderShipped>,
    //IHandleMessages<CompleteOrder>
{
    static ILog log = LogManager.GetLogger<OrderSaga>();

    public OrderSaga()
    {
        //DefaultFactory defaultFactory = LogManager.Use<DefaultFactory>();
        //defaultFactory.Directory("pathToLoggingDirectory");
    }
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderSagaData> mapper)
    {
     
        mapper.ConfigureMapping<OrderPlaced>(message => message.OrderId)
            .ToSaga(sagaData => sagaData.OrderId);
        //mapper.ConfigureMapping<OrderShipped>(message => message.OrderId)
        //    .ToSaga(sagaData => sagaData.OrderId);
        //mapper.ConfigureMapping<OrderQuantityChecked>(message => message.OrderId)
        //    .ToSaga(sagaData => sagaData.OrderId);
        //mapper.ConfigureMapping<OrderBilled>(message => message.OrderId)
        //    .ToSaga(sagaData => sagaData.OrderId);
        //mapper.ConfigureMapping<CompleteOrder>(message => message.OrderId)
        //    .ToSaga(sagaData => sagaData.OrderId);
       
    }

    public async Task Handle(OrderPlaced message, IMessageHandlerContext context)
    {
   
        var completeOrder = new CompleteOrder
        {
            OrderId = Data.OrderId
        };
        //var sendOptions = new SendOptions();
        //sendOptions.DelayDeliveryWith(TimeSpan.FromSeconds(10));
        //sendOptions.RouteToThisEndpoint();
        //await context.Send(completeOrder, sendOptions)
        //    .ConfigureAwait(false);
        Data.IsPlaced = true;
        var timeout = DateTime.UtcNow.AddSeconds(15);

        //await RequestTimeout<CancelOrder>(context, timeout)
        //    .ConfigureAwait(false);

    }

    public Task Handle(CompleteOrder message, IMessageHandlerContext context)
    {
        log.Info($"CompleteOrder received with OrderId {message.OrderId}");
        Data.IsComplete = true;
        //   MarkAsComplete();
        return Task.CompletedTask;
    }

    public Task Handle(OrderShipped message, IMessageHandlerContext context)
    {
        log.Info($"OrderShipped received with OrderId {message.OrderId}");
        Data.IsShipped = true;
        //   MarkAsComplete();
        return Task.CompletedTask;
    }

    //public Task Timeout(CancelOrder state, IMessageHandlerContext context)
    //{
    //    log.Info($"CompleteOrder not received soon enough OrderId {Data.OrderId}. Calling MarkAsComplete");
    //    Data.IsCanceled = true;
    //    //MarkAsComplete();
    //    return Task.CompletedTask;
    //}

    public async Task Handle(OrderBilled message, IMessageHandlerContext context)
    {
        Data.IsBilled = true;
        await CompleteOrder(context);
    }

    public async Task Handle(OrderQuantityChecked message, IMessageHandlerContext context)
    {
        Data.IsQuantityChecked = true;
        await CompleteOrder(context);

    }

    private async Task CompleteOrder(IMessageHandlerContext context)
    {
        if (Data.IsBilled && Data.IsQuantityChecked && Data.IsShipped)
        {
            Data.IsComplete = true;
            MarkAsComplete();
            //   await context.Publish(new OrderComplited()).ConfigureAwait(false);
        }
        //  Task.CompletedTask;

    }
}