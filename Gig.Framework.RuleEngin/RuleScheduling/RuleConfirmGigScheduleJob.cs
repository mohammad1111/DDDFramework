using Gig.Framework.Scheduling;
using Quartz;

namespace Gig.Framework.RuleEngine.RuleScheduling;

public class RuleConfirmGigScheduleJob : GigScheduleJob<RuleExpireJob>
{
    public override ITrigger GetSchedule(TriggerBuilder builder)
    {
        return builder.WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever()).Build();
    }
}