using Gig.Framework.Core.Events;

namespace Gig.Framework.Core.UserContexts;

public interface IUserContextService
{
    
    public Guid Id { get; set; }
    IUserContext UserContext { get; }

    void SetUserContext(string token);

    void SetUserContext(IUserContext userContext);
    void SetUserContext(IEvent @event);
}