using System;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.JobScheduler.Domain;
using Nop.Plugin.JobScheduler.Services;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;

namespace Nop.Plugin.JobScheduler.Common
{
    public static class SchedulerHelper
    {
        /// <summary>
        /// 初始化作业
        /// </summary>
        /// <param name="schedulerr"></param>
        /// <param name="schedulerType"></param>
        /// <param name="timeInterval"></param>
        /// <param name="intervalValue"></param>
        public static void InitQuartzScheduler(this IScheduler schedulerr, Type schedulerType, TimeInterval timeInterval, int intervalValue)
        {
            var schedulerTypeName = schedulerType.Name;
            var schedulerTriggerSuffix = SchedulerConstant.TriggerSuffix;

            switch (timeInterval)
            {
                case TimeInterval.Second:
                    schedulerr.ScheduleJob(new JobDetailImpl(schedulerTypeName, schedulerType), new CalendarIntervalTriggerImpl(schedulerTypeName + schedulerTriggerSuffix, IntervalUnit.Second, intervalValue));
                    break;
                case TimeInterval.Minute:
                    schedulerr.ScheduleJob(new JobDetailImpl(schedulerTypeName, schedulerType), new CalendarIntervalTriggerImpl(schedulerTypeName + schedulerTriggerSuffix, IntervalUnit.Minute, intervalValue));
                    break;
                case TimeInterval.Hour:
                    schedulerr.ScheduleJob(new JobDetailImpl(schedulerTypeName, schedulerType), new CalendarIntervalTriggerImpl(schedulerTypeName + schedulerTriggerSuffix, IntervalUnit.Hour, intervalValue));
                    break;
                case TimeInterval.Day:
                    schedulerr.ScheduleJob(new JobDetailImpl(schedulerTypeName, schedulerType), new CalendarIntervalTriggerImpl(schedulerTypeName + schedulerTriggerSuffix, IntervalUnit.Day, intervalValue));
                    break;
                case TimeInterval.Month:
                    schedulerr.ScheduleJob(new JobDetailImpl(schedulerTypeName, schedulerType), new CalendarIntervalTriggerImpl(schedulerTypeName + schedulerTriggerSuffix, IntervalUnit.Month, intervalValue));
                    break;
                case TimeInterval.Year:
                    schedulerr.ScheduleJob(new JobDetailImpl(schedulerTypeName, schedulerType), new CalendarIntervalTriggerImpl(schedulerTypeName + schedulerTriggerSuffix, IntervalUnit.Year, intervalValue));
                    break;
            }
        }

        /// <summary>
        /// 根据作业系统名更新该作业执行时间
        /// </summary>
        /// <param name="schedulerFullName">作业系统名</param>
        public static void UpdateSchedulerRunTime(string schedulerFullName)
        {
            var schedulerService = EngineContext.Current.Resolve<ISchedulerService>();
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            var scheduler = schedulerService.GetSchedulerBySystemName(schedulerFullName);

            if (scheduler != null)
            {
                var updatedSchedulerCommandText = scheduler.RunJobTime.HasValue
                      ? string.Format("update {0} set LastRunTime='{1}',RunJobTime='{2}' where Id ={3}", SchedulerConstant.SchedulerTableName, scheduler.RunJobTime.Value, DateTime.Now, scheduler.Id)
                      : string.Format("update {0} set RunJobTime='{1}' where Id ={2}", SchedulerConstant.SchedulerTableName, DateTime.Now, scheduler.Id);

                dbContext.ExecuteSqlCommand(updatedSchedulerCommandText, true);
            }
        }
    }
}
