using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Plugin.JobScheduler.Domain;

namespace Nop.Plugin.JobScheduler.Services
{
    public interface IVisitCustomerService
    {
        /// <summary>
        /// 获取当天（匿名/注册）的数量
        /// </summary>
        /// <param name="createdFromTime">开始时间</param>
        /// <param name="createdToTime">结束时间</param>
        /// <param name="shouldFilter">是否有过滤条件</param>
        /// <returns></returns>
        int GetVisitCustomerCount(DateTime createdFromTime, DateTime createdToTime, bool shouldFilter = true);

        /// <summary>
        /// 获取某天用户列表
        /// </summary>
        /// <param name="createdFromTime">开始时间</param>
        /// <param name="createdToTime">结束时间</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="shouldFilter">是否有过滤条件</param>
        /// <returns></returns>
        PagedList<VisitCustomer> GetAllVisitCustomerList(DateTime createdFromTime, DateTime createdToTime, int pageIndex = 0,
            int pageSize = int.MaxValue, bool shouldFilter = true);
    }

    public class VisitCustomerService : IVisitCustomerService
    {
        private readonly IRepository<VisitCustomer> _repository;

        public VisitCustomerService(IRepository<VisitCustomer> repository)
        {
            _repository = repository;
        }

        public int GetVisitCustomerCount(DateTime createdFromTime, DateTime createdToTime, bool shouldFilter = true)
        {
            var query = _repository.TableNoTracking;

            if (shouldFilter)
                query = query.Where(x => createdFromTime <= x.VisitedTime && createdToTime >= x.VisitedTime);

            var visitCustomerCount = query.Count();

            return visitCustomerCount;
        }

        public PagedList<VisitCustomer> GetAllVisitCustomerList(DateTime createdFromTime, DateTime createdToTime, int pageIndex = 0, int pageSize = int.MaxValue, bool shouldFilter = true)
        {
            var query = _repository.TableNoTracking;

            if (shouldFilter)
                query = query.Where(x => createdFromTime <= x.VisitedTime && createdToTime >= x.VisitedTime);

            query = query.OrderByDescending(x => x.VisitedTime);

            var visitCustomerList = new PagedList<VisitCustomer>(query, pageIndex, pageSize);

            return visitCustomerList;
        }
    }
}
