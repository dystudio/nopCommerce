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
        /// <param name="settings">Clickatell settings</param>
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

            //using (var smsClient = new ClickatellSmsClient(new BasicHttpBinding(), new EndpointAddress("http://api.clickatell.com/soap/document_literal/webservice")))
            //{
            //    //check credentials
            //    var authentication = smsClient.auth(int.Parse(clickatellSettings.ApiId), clickatellSettings.Username, clickatellSettings.Password);
            //    if (!authentication.ToUpperInvariant().StartsWith("OK"))
            //    {
            //        _logger.Error(string.Format("Clickatell SMS error: {0}", authentication));
            //        return false;
            //    }

            //    //send SMS
            //    var sessionId = authentication.Substring(4);
            //    var result = smsClient.sendmsg(sessionId, int.Parse(clickatellSettings.ApiId), clickatellSettings.Username, clickatellSettings.Password,
            //        text, new [] { clickatellSettings.PhoneNumber }, string.Empty, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            //        string.Empty, 0, string.Empty, string.Empty, string.Empty, 0).FirstOrDefault();

            //    if (result == null || !result.ToUpperInvariant().StartsWith("ID"))
            //    {
            //        _logger.Error(string.Format("Clickatell SMS error: {0}", result));
            //        return false;
            //    }
            //}

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
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.ApiId", "API ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.ApiId.Hint", "Specify Alidayu API ID.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.Enabled", "Enabled");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.Enabled.Hint", "Check to enable SMS provider.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.Password", "Password");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.Password.Hint", "Specify Alidayu API password.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.PhoneNumber", "Phone number");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.PhoneNumber.Hint", "Enter your phone number.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.TestMessage", "Message text");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.TestMessage.Hint", "Enter text of the test message.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.Username", "Username");
            this.AddOrUpdatePluginLocaleResource("Plugins.Sms.Alidayu.Fields.Username.Hint", "Specify Alidayu API username.");
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
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.ApiId");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.ApiId.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.Enabled");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.Enabled.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.Password");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.Password.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.PhoneNumber");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.PhoneNumber.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.TestMessage");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.TestMessage.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.Username");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.Fields.Username.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.SendTest");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.SendTest.Hint");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.TestFailed");
            this.DeletePluginLocaleResource("Plugins.Sms.Alidayu.TestSuccess");

            base.Uninstall();
        }

        #endregion
    }
}
