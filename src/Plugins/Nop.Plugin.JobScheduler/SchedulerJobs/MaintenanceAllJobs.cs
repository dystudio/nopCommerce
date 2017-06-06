using System;
using System.Linq;
using Nop.Core.Infrastructure;
using Nop.Plugin.JobScheduler.Common;
using Nop.Plugin.JobScheduler.Services;
using Quartz;

namespace Nop.Plugin.JobScheduler.SchedulerJobs
{
    public class MaintenanceAllJobs : IJob
    {
        private readonly ISchedulerService _schedulerService;

        public MaintenanceAllJobs(ISchedulerService schedulerService)
        {
            _schedulerService = schedulerService;
        }

        public void Execute(IJobExecutionContext context)
        {
            var allSchedulerList = _schedulerService.GetSchedulers().ToList();

            var scheduler = EngineContext.Current.Resolve<IScheduler>();

            foreach (var item in allSchedulerList)
            {
                if (!item.Enabled || item.Deleted)
                {
                    var type = Type.GetType(item.SystemName);
                    if (type != null)
                    {
                        JobKey jobKey = new JobKey(type.Name);
                        scheduler.DeleteJob(jobKey);
                    }
                }
            }

            var scheduleFullName = typeof(MaintenanceAllJobs).FullName;
            SchedulerHelper.UpdateSchedulerRunTime(scheduleFullName);
        }
    }
}
