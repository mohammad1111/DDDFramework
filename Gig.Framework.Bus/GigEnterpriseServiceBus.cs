using Gig.Framework.Core.Events;
using Gig.Framework.Core.Helper;
using Gig.Framework.Core.Serilizer;
using MassTransit;
using Serilog;

namespace Gig.Framework.Bus;

public class GigEnterpriseServiceBus : IEnterpriseServiceBus
{
    private readonly IBusControl _bus;
    private readonly ICrcEngine _crcEngine;
    private readonly ILogger _logger;
    private readonly ISerializer _serializer;

    public GigEnterpriseServiceBus(IBusControl bus, ILogger logger, ISerializer serializer, ICrcEngine crcEngine)
    {
        _bus = bus;
        _logger = logger;
        _serializer = serializer;
        _crcEngine = crcEngine;
    }

    public async Task Send(object message)
    {
        await _bus.Send(message).ConfigureAwait(false);
        _logger.Information("Send Event To Broker :{Message}", _serializer.Serialize(message));
    }

    public async Task Publish(object message)
    {
        string crc = null;
        if (message is IEvent) crc = _crcEngine.GenerateCheckSum(message);
        var eventPriorityType = typeof(EventPriority);
        var priority =
            (EventPriority[])eventPriorityType.GetCustomAttributes(eventPriorityType, false);   
        await _bus.Publish(message, context =>
        {
            if (!string.IsNullOrEmpty(crc))
            {
                context.Headers.Set("crc", crc);
            }

            if (priority.Any())
            {
                context.SetPriority(priority.First().Priority);
            }
        });
        _logger.Information("Publish Event To Broker :{Message}", _serializer.Serialize(message));
    }
}