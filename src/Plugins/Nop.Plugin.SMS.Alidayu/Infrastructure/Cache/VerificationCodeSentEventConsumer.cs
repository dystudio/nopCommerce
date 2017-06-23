using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Plugin.SMS.Alidayu.Domain;
using Nop.Services.Events;

namespace Nop.Plugin.SMS.Alidayu.Infrastructure.Cache
{
    public class VerificationCodeSentEventConsumer : IConsumer<VerificationCodeSentEvent>
    {
        #region Fields

        private ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public VerificationCodeSentEventConsumer(ICacheManager cacheManager)
        {
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static"); ;
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

        }

        #endregion
    }
}