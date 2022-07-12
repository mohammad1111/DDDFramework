using Gig.Framework.Core.Events;
using Gig.Framework.Core.Security;

namespace Gig.Framework.Core.UserContexts;

public class UserContextService : IUserContextService
{
    private readonly ISecurityManager _securityManager;

    public UserContextService(ISecurityManager securityManager)
    {
        _securityManager = securityManager;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public IUserContext UserContext { get; private set; }

    public void SetUserContext(string token)
    {
        UserContext = _securityManager.Validate(token);
    }

    public void SetUserContext(IUserContext userContext)
    {
        UserContext = userContext;
    }

    public void SetUserContext(IEvent @event)
    {
        UserContext =
            new UserContext("", @event.UserId, @event.CompanyId,
                @event.BranchId, "", @event.CompanyId.ToString(), false, @event.LangTypeCode, @event.SubSystemId);
    }
}