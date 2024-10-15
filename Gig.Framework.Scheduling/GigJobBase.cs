using System.Threading.Tasks;
using Gig.Framework.Core;
using Quartz;
using Serilog;

namespace Gig.Framework.Scheduling;

public abstract class GigJobBase : IGigJob
{
    public readonly ILogger Logger;
    public readonly IRequestContext RequestContext;

    protected GigJobBase(ILogger logger, IRequestContext requestContext)
    {
        Logger = logger;
        RequestContext = requestContext;
    }

    public async Task ExecuteJob(IJobExecutionContext gigJobExecutionContext)
    {
        await Execute(gigJobExecutionContext);
        Logger.Information("Run {Job} Completed, {TraceId}", GetType(), RequestContext.GetUserContext().TraceId);
    }

    protected abstract Task Execute(IJobExecutionContext gigJobExecutionContext);
}