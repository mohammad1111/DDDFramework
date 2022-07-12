using MassTransit;

namespace Gig.Framework.EventBus.EndpointNameFormatterExtensions;

public static class CustomConfigurationExtensions
{
    public static void ApplyCustomBusConfiguration(this IBusFactoryConfigurator configurator)
    {
        var entityNameFormatter = configurator.MessageTopology.EntityNameFormatter;

        configurator.MessageTopology.SetEntityNameFormatter(new CustomEntityNameFormatter(entityNameFormatter));
    }

    public static void ApplyCustomMassTransitConfiguration(this IBusRegistrationConfigurator configurator)
    {
        configurator.SetEndpointNameFormatter(new CustomEndpointNameFormatter());
    }
}