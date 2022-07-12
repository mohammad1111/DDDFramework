namespace Gig.Framework.Core.Security;

public interface IAuthorizationService
{
    void Authorize(params int[] operations);
}