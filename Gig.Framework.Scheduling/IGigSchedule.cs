using Quartz;

namespace Gig.Framework.Scheduling;

public interface IGigSchedule
{
    public Type JobType { get; }

    public ITrigger GetSchedule(TriggerBuilder builder);
}