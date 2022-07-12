using System.Collections.Specialized;
using System.Reflection;
using Gig.Framework.Core.DependencyInjection;
using Gig.Framework.Core.Settings;
using Quartz;
using Quartz.Impl;

namespace Gig.Framework.Scheduling;

public class GigScheduler : IGigScheduler
{
    private IDataSetting _setting;

    public Task Run(IEnumerable<Assembly> assemblies)
    {
        throw new NotImplementedException();
    }

    public async Task StartGigScheduler(IGigContainer container)
    {
        _setting = container.Resolve<IDataSetting>();
        var settings = new NameValueCollection();
        settings.Set("quartz.scheduler.instanceName", _setting.MicroServiceName);
        ISchedulerFactory sf = new StdSchedulerFactory(settings);
        var sc = await sf.GetScheduler();
        try
        {
            var gigSchedules = container.ResolveAll<IGigSchedule>();

            foreach (var schedule in gigSchedules)
            {
                var detail = JobBuilder.Create(schedule.JobType)
                    .WithIdentity(schedule.JobType.FullName ?? schedule.GetType().ToString()).Build();
                var trigger = schedule.GetSchedule(TriggerBuilder.Create());
                await sc.ScheduleJob(detail, trigger);
                var delay = TimeSpan.FromSeconds(5);
                await sc.StartDelayed(delay);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}