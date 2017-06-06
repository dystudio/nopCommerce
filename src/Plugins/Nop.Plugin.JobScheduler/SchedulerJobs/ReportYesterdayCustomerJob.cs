using System;
using System.Linq;
using System.Text;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.JobScheduler.Common;
using Nop.Plugin.JobScheduler.Services;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Quartz;

namespace Nop.Plugin.JobScheduler.SchedulerJobs
{
    /// <summary>
    /// 统计昨天(匿名/注册)信息
    /// </summary>
    public class ReportYesterdayCustomerJob : IJob
    {
        private readonly IDbContext _dbContext;

        private readonly IRepository<Customer> _customeRepository;

        private readonly ISchedulerService _schedulerService;

        public ReportYesterdayCustomerJob(IDbContext dbContext,
            IRepository<Customer> customeRepository,
            ISchedulerService schedulerService)
        {
            _dbContext = dbContext;
            _customeRepository = customeRepository;
            _schedulerService = schedulerService;
        }

        public void Execute(IJobExecutionContext context)
        {
            var yesterday = DateTime.Now.AddDays(-1); // 设置昨天为搜索时间

            var createdDateTimeFrom = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 00, 00, 00);
            var createdDateTimeTo = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59);

            // 获取昨天（匿名/注册)用户信息
            var newCustomerList = _customeRepository.TableNoTracking
               .Where(x => createdDateTimeFrom <= x.CreatedOnUtc && createdDateTimeTo >= x.CreatedOnUtc)
               .ToList()
               .Select(x => new
               {
                   x.Id,
                   x.Username,
                   x.Email,
                   RealName = x.GetFullName(),
                   VisitIpAddress = x.LastIpAddress,
                   IsRegisterCustomer = x.IsRegistered(),
                   VisitedTime = x.CreatedOnUtc
               }).ToList();

            var schedulerFullName = typeof(ReportYesterdayCustomerJob).FullName;
            var scheduler = _schedulerService.GetSchedulerBySystemName(schedulerFullName);

            if (scheduler != null && scheduler.Enabled && !scheduler.Deleted)
            {
                SchedulerHelper.UpdateSchedulerRunTime(schedulerFullName);

                var commandText = new StringBuilder();
                foreach (var item in newCustomerList)
                {
                    commandText.Append(string.Format(@"insert into [dbo].[{0}] (CustomerId,Username,RealName,Email,IsRegisterCustomer,VisitIpAddress,VisitedTime) values ({1},'{2}','{3}','{4}',{5},'{6}','{7}');",
                        SchedulerConstant.VisitedCustomerTableName, item.Id, item.Username, item.RealName, item.Email, item.IsRegisterCustomer ? 1 : 0, item.VisitIpAddress, item.VisitedTime));
                }
                if (!string.IsNullOrEmpty(commandText.ToString()))
                    _dbContext.ExecuteSqlCommand(commandText.ToString());
            }
            else
            {
                EngineContext.Current.Resolve<ILogger>().InsertLog(LogLevel.Information, "BackupNewCustomerJob", "备份昨天(匿名/注册)信息作业未正常启用");
            }
        }
    }
}
