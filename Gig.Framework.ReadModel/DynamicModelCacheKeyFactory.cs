using Gig.Framework.Core;
using Gig.Framework.Core.Settings;
using Gig.Framework.Core.UserContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Gig.Framework.ReadModel;

public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool isRuntime)
    {
        if (!(context is ReadDbContext readDbContext))
        {
            return context.GetType();
        }

        if (!isRuntime)
        {
            readDbContext.Database.GetService<IUserContextService>().SetUserContext(readDbContext.Database.GetService<IDataSetting>().SystemToken);
        }

        var userContext = readDbContext.Database.GetService<IRequestContext>().GetUserContext();
        return (userContext.UserId, userContext.CompanyId, userContext.BranchId, isRuntime);
    }
}