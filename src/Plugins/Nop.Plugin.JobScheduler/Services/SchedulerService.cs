using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Plugin.JobScheduler.Domain;

namespace Nop.Plugin.JobScheduler.Services
{
    public interface ISchedulerService
    {
        Scheduler GetSchedulerById(int id);

        Scheduler GetSchedulerBySystemName(string systemName);

        /// <summary>
        /// 获取所有作业列表
        /// </summary>
        /// <returns></returns>
        IQueryable<Scheduler> GetSchedulers();

        /// <summary>
        /// 批量添加作业
        /// </summary>
        /// <param name="schedulerList"></param>
        void CreateSchedulerBatch(List<Scheduler> schedulerList);

        /// <summary>
        /// 更新作业
        /// </summary>
        /// <param name="scheduler"></param>
        void UpdateScheduler(Scheduler scheduler);

        /// <summary>
        /// 删除作业
        /// </summary>
        /// <param name="scheduler"></param>
        void DeleteScheduler(Scheduler scheduler);

        /// <summary>
        /// 批量删除作业
        /// </summary>
        /// <param name="schedulerList"></param>
        void DeleteSchedulerBatch(List<Scheduler> schedulerList);
    }

    public class SchedulerService : ISchedulerService
    {
        private readonly IRepository<Scheduler> _schedulerRepository;

        public SchedulerService(IRepository<Scheduler> schedulerRepository)
        {
            _schedulerRepository = schedulerRepository;
        }

        public Scheduler GetSchedulerById(int id)
        {
            if (id <= 0)
                return null;

            return _schedulerRepository.GetById(id);
        }

        public Scheduler GetSchedulerBySystemName(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                return null;

            return _schedulerRepository.TableNoTracking.SingleOrDefault(x => x.SystemName == systemName);
        }

        public IQueryable<Scheduler> GetSchedulers()
        {
            var query = _schedulerRepository.Table;

            return query;
        }

        public void CreateSchedulerBatch(List<Scheduler> schedulerList)
        {
            if (schedulerList.Any())
            _schedulerRepository.Insert(schedulerList);
        }

        public void UpdateScheduler(Scheduler scheduler)
        {
            if (scheduler == null)
                throw new ArgumentNullException("scheduler");

            _schedulerRepository.Update(scheduler);
        }

        public void DeleteScheduler(Scheduler scheduler)
        {
            if (scheduler == null)
                throw new ArgumentNullException("scheduler");

            scheduler.Deleted = true;
            _schedulerRepository.Update(scheduler);
        }

        public void DeleteSchedulerBatch(List<Scheduler> schedulerList)
        {
            if (schedulerList.Any())
                _schedulerRepository.Update(schedulerList);
        }
    }
}
