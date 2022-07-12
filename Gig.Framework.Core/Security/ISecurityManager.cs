namespace Gig.Framework.Core.Security;

public interface ISecurityManager
{
    IUserContext Validate(string token);

    IUserContext Read(string token);

    IUserContext ValidateWithoutExpireTime(string token);

    Task<int> GetAccessLevel(long userId, long companyId, string permissions);

    Task<int> GetAccessLevel(long userId, long companyId, string[] permissions);

    string CreateAccessToken(TokenClaimModel model, BearerTokenOptionsModel options);

    string GetAccessToken(TokenClaimModel model, BearerTokenOptionsModel options);
}