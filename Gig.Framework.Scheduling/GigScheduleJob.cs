using System;
using Quartz;

namespace Gig.Framework.Scheduling;

public abstract class GigScheduleJob<TJob> : IGigSchedule
    where TJob : IGigJob
{
    public Type JobType => typeof(GigJobMiddleware<TJob>);

    /// <summary>
    ///     Return Schedule Setting
    /// </summary>
    /// <param name="builder">builder all ready is created</param>
    /// <returns>Schedule</returns>
    public abstract ITrigger GetSchedule(TriggerBuilder builder);
}