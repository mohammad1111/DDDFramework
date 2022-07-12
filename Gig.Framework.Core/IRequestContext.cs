namespace Gig.Framework.Core;

public interface IRequestContext
{
    IUserContext GetUserContext();

    string GetToken();
    
    Task<bool> HasPermission(string[] operations);

    Task<int> GetAccessLevel();
}