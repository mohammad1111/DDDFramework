using Gig.Framework.Core.Events;

namespace Gig.Framework.Application;

public interface ICommandHandler
{
    IList<IEvent> Events { get; }
}