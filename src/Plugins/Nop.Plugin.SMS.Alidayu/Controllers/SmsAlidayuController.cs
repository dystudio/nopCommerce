using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Plugin.SMS.Alidayu.Models;
using Nop.Plugin.SMS.Alidayu;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.SMS.Alidayu.Controllers
{
    [AdminAuthorize]
    public class SmsAlidayuController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SmsAlidayuController(ILocalizationService localizationService,
            IPluginFinder pluginFinder,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext)
        {
            this._localizationService = localizationService;
            this._pluginFinder = pluginFinder;
            this._settingService = settingService;            
            this._storeService = storeService;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var clickatellSettings = _settingService.LoadSetting<AlidayuSettings>(storeScope);

            var model = new SmsAlidayuModel
            {
                Enabled = clickatellSettings.Enabled,
                ApiId = clickatellSettings.ApiId,
                Password = clickatellSettings.Password,
                Username = clickatellSettings.Username,
                PhoneNumber = clickatellSettings.PhoneNumber,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.Enabled_OverrideForStore = _settingService.SettingExists(clickatellSettings, x => x.Enabled, storeScope);
                model.PhoneNumber_OverrideForStore = _settingService.SettingExists(clickatellSettings, x => x.PhoneNumber, storeScope);
            }

            return View("~/Plugins/SMS.Clickatell/Views/Configure.cshtml", model);
        }

        [ChildActionOnly]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public ActionResult Configure(SmsAlidayuModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var clickatellSettings = _settingService.LoadSetting<AlidayuSettings>(storeScope);

            //save settings
            clickatellSettings.Enabled = model.Enabled;
            clickatellSettings.ApiId = model.ApiId;
            clickatellSettings.Username = model.Username;
            clickatellSettings.Password = model.Password;
            clickatellSettings.PhoneNumber = model.PhoneNumber;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSetting(clickatellSettings, x => x.ApiId, storeScope, false);
            _settingService.SaveSetting(clickatellSettings, x => x.Username, storeScope, false);
            _settingService.SaveSetting(clickatellSettings, x => x.Password, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(clickatellSettings, x => x.Enabled, model.Enabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(clickatellSettings, x => x.PhoneNumber, model.PhoneNumber_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("test")]
        public ActionResult TestSms(SmsAlidayuModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("Mobile.SMS.Clickatell");
            if (pluginDescriptor == null)
                throw new Exception("Cannot load the plugin");

            var plugin = pluginDescriptor.Instance() as AlidayuSmsProvider;
            if (plugin == null)
                throw new Exception("Cannot load the plugin");

            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var alidayuSettings = _settingService.LoadSetting<AlidayuSettings>(storeScope);

            //test SMS send
            if (plugin.SendSms(model.TestMessage, 0, alidayuSettings))
                SuccessNotification(_localizationService.GetResource("Plugins.Sms.Clickatell.TestSuccess"));
            else
                ErrorNotification(_localizationService.GetResource("Plugins.Sms.Clickatell.TestFailed"));

            return Configure();
        }

        #endregion
    }
}