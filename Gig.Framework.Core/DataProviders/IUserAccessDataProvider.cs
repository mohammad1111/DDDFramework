namespace Gig.Framework.Core.DataProviders;

public interface IUserAccessDataProvider
{
    Task<int> GetAccessLevel(long userId, long companyId, string[] permissions);
}