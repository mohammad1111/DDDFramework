using Gig.Framework.Core.DependencyInjection;
using Gig.Framework.Core.Security;
using Gig.Framework.Core.Settings;
using Gig.Framework.Core.UserContexts;
using Quartz;

namespace Gig.Framework.Scheduling;

[DisallowConcurrentExecution]
public class GigJobMiddleware<T> : IJob where T : IGigJob
{
    private readonly IDataSetting _dataSetting;
    private readonly ISecurityManager _securityManager;
    private readonly IServiceLocator _serviceLocator;

    public GigJobMiddleware(IServiceLocator serviceLocator, ISecurityManager securityManager, IDataSetting dataSetting)
    {
        _serviceLocator = serviceLocator;
        _securityManager = securityManager;
        _dataSetting = dataSetting;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        if (_serviceLocator.Current != null)
        {
            using var scope = _serviceLocator.Current.CreateScope();

            //todo:Change this
            var token = _serviceLocator.Current.Resolve<IUserContextService>();

            token.SetUserContext(_securityManager.Read(_dataSetting.SystemToken));
            await _serviceLocator.Current.Resolve<T>().ExecuteJob(context);
        }
    }
}