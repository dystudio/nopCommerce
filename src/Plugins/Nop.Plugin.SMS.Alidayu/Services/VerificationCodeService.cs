using Nop.Core;
using Nop.Plugin.SMS.Alidayu.Domain;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;
using Nop.Plugin.SMS.Alidayu.Models;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.SMS.Alidayu.Services
{
    public class VerificationCodeService : IVerificationCodeService
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion
        public VerificationCodeService(
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext,
            ILogger logger,
            IEventPublisher eventPublisher,
            ICacheManager cacheManager)
        {
          
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._logger = logger;
            this._eventPublisher = eventPublisher;
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
        }

        public bool SendVerificationCode(int storeScope, string phoneNumber)
        {
            var number = this.GenerateNumber(6, true);
            //load settings for a chosen store scope
            var alidayuSettings = _settingService.LoadSetting<AlidayuSettings>(storeScope);

            var url = string.Empty;
            if (alidayuSettings.SslEnabled)
            {
                url = alidayuSettings.SandboxEnabled ?
                    AlidayuRequestUrl.SandboxHttpsRequestUrl : AlidayuRequestUrl.HttpsRequestUrl;
            }
            else
            {
                url = alidayuSettings.SandboxEnabled ?
                   AlidayuRequestUrl.SandboxHttpRequestUrl : AlidayuRequestUrl.HttpRequestUrl;
            }

            ITopClient client = new DefaultTopClient(url, alidayuSettings.AppKey, alidayuSettings.AppSecret);
            AlibabaAliqinFcSmsNumSendRequest req = new AlibabaAliqinFcSmsNumSendRequest();
            req.Extend = "123456";
            req.SmsType = "normal";
            req.SmsFreeSignName = alidayuSettings.SmsFreeSignName;
            //验证码${code}，您正在进行${product}身份验证，打死不要告诉别人哦！
            //New order ${orderId} was placed for the total amount ${OrderTotal}
            //请创建短信消息模板：新订单 ${orderId}成功下单，订单总额 ${orderTotal}。
            req.SmsParam = "{\"code\":\"" + number.ToString() + "\",\"product\":\"" + alidayuSettings.ProductName + "\"}";
            req.RecNum = phoneNumber;
            req.SmsTemplateCode = alidayuSettings.SmsTemplateCodeForVerificationCode;
            AlibabaAliqinFcSmsNumSendResponse response = client.Execute(req);
            if (response.IsError)
            {
                _logger.Error(string.Format("Alidayu SMS error,ErrCode: {0}, ErrMsg: {1}", response.ErrCode, response.ErrMsg));
                _logger.Error(string.Format("Alidayu SMS error,SubErrCode: {0},SubErrMsg: {1}", response.SubErrCode, response.SubErrMsg));
                return false;
            }
            else
            {
                //raise event       
                _eventPublisher.Publish(new VerificationCodeSentEvent(phoneNumber, number.ToString()));
                return true;
            }
        }

        public bool VerifyCode(VerifiedCodeModel verifiedCodeModel)
        {
            var cachedNumber = _cacheManager.Get<string>(string.Format("Nop.Plugin.SMS.Alidayu.{0}", verifiedCodeModel.PhoneNumber));
            if (!string.IsNullOrEmpty(cachedNumber) && cachedNumber == verifiedCodeModel.Number)
            {
                //raise event       
                _eventPublisher.Publish(new VerifiedCodeEvent(verifiedCodeModel.PhoneNumber, verifiedCodeModel.Number));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        private string GenerateNumber(int Length, bool Sleep)
        {
            if (Sleep) System.Threading.Thread.Sleep(3);
            string result = "";
            Random random = new Random();
            for (int i = 0; i < Length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }
    }
}
