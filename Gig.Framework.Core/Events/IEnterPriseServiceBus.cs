namespace Gig.Framework.Core.Events;

public interface IEnterpriseServiceBus
{
    Task Publish(object message);

    Task Send(object message);
}