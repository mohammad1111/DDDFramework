using System.Threading.Tasks;

namespace Gig.Framework.Bus;

public interface IMessageHandler
{
}

public interface IMessageHandler<T> : IMessageHandler
    where T : IMessage
{
    Task Handle(T message);
}