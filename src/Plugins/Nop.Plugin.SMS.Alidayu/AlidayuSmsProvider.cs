using System;
using System.Linq;
using System.ServiceModel;
using System.Web.Routing;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Plugin.SMS.Alidayu;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;

namespace Nop.Plugin.SMS.Alidayu
{
    /// <summary>
    /// Represents the Alidayu SMS provider
    /// </summary>
    public class AlidayuSmsProvider : BasePlugin, IMiscPlugin
    {
        #region Fields

        private readonly AlidayuSettings _alidayuSettings;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public AlidayuSmsProvider(AlidayuSettings alidayuSettings,
            ILogger logger,
            IOrderService orderService,
            ISettingService settingService)
        {
            this._alidayuSettings = alidayuSettings;
            this._logger = logger;
            this._orderService = orderService;
            this._settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send SMS 
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="orderId">Order id</param>
        /// <param name="settings">alidayu settings</param>
        /// <returns>True if SMS was successfully sent; otherwise false</returns>
        public bool SendSms(string text, int orderId, AlidayuSettings settings = null)
        {
            var alidayuSettings = settings ?? _alidayuSettings;
            if (!alidayuSettings.Enabled)
                return false;

            //change text
            var order = _orderService.GetOrderById(orderId);
            if (order != null)
                text = string.Format("New order #{0} was placed for the total amount {1:0.00}", order.Id, order.OrderTotal);

            var url = string.Empty;
            if (alidayuSettings.SslEnabled)
            {
                url = alidayuSettings.SandboxEnabled? 
                    AlidayuRequestUrl.SandboxHttpsRequestUrl: AlidayuRequestUrl.HttpsRequestUrl;
            }
            else
            {
                url = alidayuSettings.SandboxEnabled ?
                   AlidayuRequestUrl.SandboxHttpRequestUrl : AlidayuRequestUrl.HttpRequestUrl;
            }

            ITopClient client = new DefaultTopClient(url, _alidayuSettings.AppKey, _alidayuSettings.AppSecret);
            AlibabaAliqinFcSmsNumSendRequest req = new AlibabaAliqinFcSmsNumSendRequest();
            req.Extend = "123456";
            req.SmsType = "normal";
            req.SmsFreeSignName = _alidayuSettings.SmsFreeSignName;
            //验证码${code}，您正在进行${product}身份验证，打死不要告诉别人哦！
            //New order ${orderId} was placed for the total amount ${OrderTotal}
            //请创建短信消息模板：新订单 ${orderId}成功下单，订单总额 ${orderTotal}。
            req.SmsParam = string.Format("{\"orderId\":\"{0}\",\"orderTotal\":\"{1}\"}", order.Id.ToString(),string.Format("{0:0.00}",order.OrderTotal));
            req.RecNum = _alidayuSettings.PhoneNumber;
            req.SmsTemplateCode = _alidayuSettings.SmsTemplateCode;
            AlibabaAliqinFcSmsNumSendResponse response = client.Execute(req);
            if (response.IsError)
            {
                _logger.Error(string.Format("Alidayu SMS error: {0}", response.ErrMsg));
            }
            else
            {
                Console.WriteLine(response.Body);
            }           

            //order note
            if (order != null)
            {
                order.OrderNotes.Add(new OrderNote()
                {
                    Note = "\"Order placed\" SMS alert (to store owner) has been sent",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _orderService.UpdateOrder(order);
            }

            return true;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "SmsAlidayu";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.SMS.Alidayu.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new AlidayuSettings());

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.Enabled", "Enabled");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.Enabled.Hint", "Check to enable SMS provider.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.ProductName", "ProductName");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.ProductName.Hint", "Product Name."); 
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.AppKey", "AppKey");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.AppKey.Hint", "Specify Alidayu API AppKey.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.AppSecret", "AppSecret");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.AppSecret.Hint", "Specify Alidayu API AppSecret.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.PhoneNumber", "Phone number");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.PhoneNumber.Hint", "Enter your phone number.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.SmsFreeSignName", "SmsFreeSignName");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.SmsFreeSignName.Hint", "Enter your SmsFreeSignName.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.SmsTemplateCode", "SmsTemplateCode");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.SmsTemplateCode.Hint", "Enter your SmsTemplateCode.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.TestMessage", "Message text");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.TestMessage.Hint", "Enter text of the test message.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.SendTest", "Send");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.SendTest.Hint", "Send test message");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.TestFailed", "Test message sending failed");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.TestSuccess", "Test message was sent");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<AlidayuSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.Enabled");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.Enabled.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.ProductName");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.ProductName.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.AppKey");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.AppKey.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.AppSecret");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.AppSecret.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.PhoneNumber");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.PhoneNumber.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.SmsFreeSignName");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.SmsFreeSignName.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.SmsTemplateCode");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.SmsTemplateCode.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.TestMessage");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.TestMessage.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.SendTest");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.SendTest.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.TestFailed");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.TestSuccess");

            base.Uninstall();
        }

        #endregion
    }
}
