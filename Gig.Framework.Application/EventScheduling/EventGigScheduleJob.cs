using Gig.Framework.Scheduling;
using Quartz;

namespace Gig.Framework.Application.EventScheduling;

public class EventGigScheduleJob : GigScheduleJob<EventPublisherJob>
{
    public override ITrigger GetSchedule(TriggerBuilder builder)
    {
        return builder.WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever()).Build();
    }
}