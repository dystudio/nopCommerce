using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Plugin.SMS.Alidayu.Domain;
using Nop.Services.Customers;
using Nop.Services.Events;
using System;
using System.Linq;

namespace Nop.Plugin.SMS.Alidayu.Infrastructure.Cache
{
    public class VerificationCodeSentEventConsumer : IConsumer<VerificationCodeSentEvent>
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public VerificationCodeSentEventConsumer(ICacheManager cacheManager,
            IRepository<ActivityLog> activityLogRepository,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IWebHelper webHelper,
            ICustomerService customerService)
        {
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
            this._activityLogRepository = activityLogRepository;
            this._activityLogTypeRepository = activityLogTypeRepository;
            this._webHelper = webHelper;
            this._customerService = customerService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(VerificationCodeSentEvent eventMessage)
        {
            //cache the verification number

            var keyValue = _cacheManager.Get(string.Format("Nop.Plugin.SMS.Alidayu.{0}", eventMessage.PhoneNumber),
                () =>
            {
                return eventMessage.Number;
            });

            //insert activity log
            var smsActivityType = _activityLogTypeRepository.Table.Where(
                x => x.SystemKeyword == "Nop.Plugin.SMS.Alidayu.VerificationCodeSent").FirstOrDefault();
            if (smsActivityType != null)
            {
                var customer = _customerService.GetCustomerById(1);
                var activity = new ActivityLog();
                activity.ActivityLogTypeId = smsActivityType.Id;
                activity.Customer = customer;
                activity.Comment = string.Format("Verification code {0} sent to {1}",eventMessage.Number, eventMessage.PhoneNumber);
                activity.CreatedOnUtc = DateTime.UtcNow;
                activity.IpAddress = _webHelper.GetCurrentIpAddress();

                _activityLogRepository.Insert(activity);
            }
            
        }

        #endregion
    }
}