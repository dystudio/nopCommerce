using System;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Logging;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.JobScheduler.Common;
using Nop.Plugin.JobScheduler.Services;
using Nop.Services.Logging;
using Quartz;

namespace Nop.Plugin.JobScheduler.SchedulerJobs
{
    public class BackupDatabaseJob : IJob
    {
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;

        private readonly ISchedulerService _schedulerService;

        public BackupDatabaseJob(IDataProvider dataProvider,
            IDbContext dbContext, ISchedulerService schedulerService)
        {
            _dataProvider = dataProvider;
            _dbContext = dbContext;
            _schedulerService = schedulerService;
        }

        public void Execute(IJobExecutionContext context)
        {
            if (_dataProvider.BackupSupported)
            {
                var schedulerFullName = typeof(BackupDatabaseJob).FullName;
                var scheduler = _schedulerService.GetSchedulerBySystemName(schedulerFullName);

                if (scheduler != null && scheduler.Enabled && !scheduler.Deleted)
                {
                    SchedulerHelper.UpdateSchedulerRunTime(schedulerFullName);

                    var fileName = string.Format(
                        "{0}database_{1}_{2}.bak",
                        CommonHelper.MapPath("~/Administration/db_backups/"),
                        DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"),
                        CommonHelper.GenerateRandomDigitCode(10));

                    var commandText = string.Format(
                        "BACKUP DATABASE [{0}] TO DISK = '{1}' WITH FORMAT",
                        _dbContext.DbName(),
                        fileName);

                    _dbContext.ExecuteSqlCommand(commandText, true);


                }
                else
                {
                    EngineContext.Current.Resolve<ILogger>().InsertLog(LogLevel.Information, "BackupDatabaseJob", "数据库备份作业未正常启动");
                    // Interrupt();
                }
            }
            else
            {
                EngineContext.Current.Resolve<ILogger>().InsertLog(LogLevel.Information, "BackupDatabaseJob", "该数据库不支持备份");
                //throw new DataException("该数据库不支持备份");
            }
        }

        /*
        public void Interrupt()
        {
            var Schedulerr = EngineContext.Current.Resolve<ISchedulerr>();

            TriggerKey triggerKey = new TriggerKey(typeof(BackupDatabaseJob).Name + "Trigger");
            var triggerState = Schedulerr.GetTriggerState(triggerKey);

            if (triggerState == TriggerState.Normal)
            {
                JobKey jobKey = new JobKey(typeof(BackupDatabaseJob).Name);

                Schedulerr.Interrupt(jobKey);
            }
        }*/
    }
}
