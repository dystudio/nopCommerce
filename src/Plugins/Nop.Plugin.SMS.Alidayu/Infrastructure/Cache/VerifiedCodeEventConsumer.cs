using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Plugin.SMS.Alidayu.Domain;
using Nop.Services.Events;

namespace Nop.Plugin.SMS.Alidayu.Infrastructure.Cache
{
    public class VerifiedCodeEventConsumer : IConsumer<VerifiedCodeEvent>
    {
        #region Fields

        private ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public VerifiedCodeEventConsumer(ICacheManager cacheManager)
        {
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static"); ;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(VerifiedCodeEvent eventMessage)
        {
            //cache the verification number
            var cachedNumber = _cacheManager.Get<string>(string.Format("Nop.Plugin.SMS.Alidayu.{0}", eventMessage.PhoneNumber));
            if (!string.IsNullOrEmpty(cachedNumber) && cachedNumber == eventMessage.Number)
            {
                _cacheManager.Remove(string.Format("Nop.Plugin.SMS.Alidayu.{0}", eventMessage.PhoneNumber));
            }
        }

        #endregion
    }
}