using Autofac;
using MassTransit;
using MassTransit.RabbitMqTransport;

namespace Gig.Framework.Bus;

internal static class GigConsumerExtensions
{
    public static void AddConsume<T>(IRabbitMqReceiveEndpointConfigurator endpoint, ContainerBuilder container)
        where T : class, IConsumer
    {
        //  endpoint.Consumer<T>(container.Kernel);
    }

    public static void ConfigConsume<T>(IRabbitMqReceiveEndpointConfigurator endpoint)
        where T : class, IConsumer, new()
    {
        endpoint.Consumer<T>();
    }
}