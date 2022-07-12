using Gig.Framework.EventBus.Bus;
using Gig.Framework.EventBus.Configs;
using Gig.Framework.EventBus.Contracts;
using Gig.Framework.EventBus.EndpointNameFormatterExtensions;
using Gig.Framework.EventBus.Filters;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Gig.Framework.EventBus;

public static class DependencyInjectionRegistrationExtensions
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection serviceCollection, BusConfig busConfig)
    {
        if (busConfig == null || busConfig.RabbitMqConfig == null ||
            string.IsNullOrWhiteSpace(busConfig.RabbitMqConfig.Password) ||
            string.IsNullOrWhiteSpace(busConfig.RabbitMqConfig.ServerUrl) ||
            string.IsNullOrWhiteSpace(busConfig.RabbitMqConfig.UserName))
            throw new Exception("اطلاعات ارتباط با سرور وارد نشده است");

        serviceCollection.AddMassTransit(busConfiguration =>
        {
            busConfiguration.ApplyCustomMassTransitConfiguration();
            busConfiguration.AddDelayedMessageScheduler();

            var types = busConfig.HandlerAssemblies == null || !busConfig.HandlerAssemblies.Any()
                ? new List<Type>()
                : busConfig.HandlerAssemblies.SelectMany(x => x.GetTypes())
                    .Where(z => z.GetInterfaces().Contains(typeof(IConsumer<>))).ToList();

            foreach (var type in types) busConfiguration.AddConsumer(type);

            busConfiguration.UsingRabbitMq(
                (
                    context,
                    config) =>
                {
                    config.AutoStart = true;
                    config.Durable = true;

                    if (busConfig.RetryConfig != null)
                        config.UseMessageRetry(
                            x =>
                            {
                                x.Interval(
                                    busConfig.RetryConfig.CountRetry,
                                    busConfig.RetryConfig.Interval);
                            });

                    config.UseDelayedMessageScheduler();
                    config.UsePublishFilter(typeof(SecurityPublishFilter<>), context);
                    if (types.Any()) config.UseConsumeFilter(typeof(ConsumeContextFilter<>), context);
                    var endpointName = $"GigAap_V1_{busConfig.EndPointName}";
                    config.ReceiveEndpoint(
                        endpointName,
                        endpoint =>
                        {
                            if (busConfig.RetryConfig != null)
                                config.UseMessageRetry(
                                    x =>
                                    {
                                        x.Interval(
                                            busConfig.RetryConfig.CountRetry,
                                            busConfig.RetryConfig.Interval);
                                    });

                            endpoint.UseInMemoryOutbox();
                            endpoint.UseMessageScope(context);
                            endpoint.ConfigureSagas(context);
                        });
                    config.UseInMemoryOutbox();
                    config.Host(
                        busConfig.RabbitMqConfig.ServerUrl,
                        host =>
                        {
                            host.Username(busConfig.RabbitMqConfig.UserName);
                            host.Password(busConfig.RabbitMqConfig.Password);
                        });
                    config.ApplyCustomBusConfiguration();
                    config.ConfigureEndpoints(context);
                });
        });

        serviceCollection.AddScoped<IGigEventBus, GigEventBus>();
        serviceCollection.AddSingleton<IGigBus, GigBus>();
        serviceCollection.AddSingleton<ICrcEngine, CrcEngine>();

        return serviceCollection;
    }
}