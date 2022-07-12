using System.Reflection;

namespace Gig.Framework.EventBus;

public class BusConfig
{
    /// <summary>
    ///     End Point Name For Per Application/Microservice
    /// </summary>
    public string EndPointName { get; set; }

    /// <summary>
    ///     Connection Config For Rabbit
    /// </summary>
    public RabbitMqConfig RabbitMqConfig { get; set; }


    /// <summary>
    ///     Retry Config
    /// </summary>
    public RetryConfig RetryConfig { get; set; }


    /// <summary>
    ///     All assemblies that includes the Handler class
    /// </summary>
    public Assembly[] HandlerAssemblies { get; set; }
}