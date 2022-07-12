using Quartz;

namespace Gig.Framework.Scheduling;

public interface IGigJob
{
    Task ExecuteJob(IJobExecutionContext gigJobExecutionContext);
}