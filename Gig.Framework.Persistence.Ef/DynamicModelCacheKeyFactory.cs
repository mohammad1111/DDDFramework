using Gig.Framework.Core;
using Gig.Framework.Core.Settings;
using Gig.Framework.Core.UserContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Gig.Framework.Persistence.Ef;

public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool isRuntime)
    {
        if (!(context is EfUnitOfWork efUnitOfWork))
        {
            return context.GetType();
        }

        if (!isRuntime)
        {
            efUnitOfWork.Database.GetService<IUserContextService>().SetUserContext(efUnitOfWork.Database.GetService<IDataSetting>().SystemToken);
        }

        var userContext = efUnitOfWork.Database.GetService<IRequestContext>().GetUserContext();
        return (userContext.UserId, userContext.CompanyId, userContext.BranchId, isRuntime);
    }
}