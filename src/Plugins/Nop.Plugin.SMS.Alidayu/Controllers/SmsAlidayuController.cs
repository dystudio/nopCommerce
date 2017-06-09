﻿using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Plugin.SMS.Alidayu.Models;
using Nop.Plugin.SMS.Alidayu;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.SMS.Alidayu.Services;

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
        private readonly IVerificationCodeService _verificationCodeService;

        #endregion

        #region Ctor

        public SmsAlidayuController(ILocalizationService localizationService,
            IPluginFinder pluginFinder,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext,
            IVerificationCodeService verificationCodeService)
        {
            this._localizationService = localizationService;
            this._pluginFinder = pluginFinder;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._verificationCodeService = verificationCodeService;
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var alidayuSettings = _settingService.LoadSetting<AlidayuSettings>(storeScope);

            var model = new SmsAlidayuModel
            {
                Enabled = alidayuSettings.Enabled,
                ProductName = alidayuSettings.ProductName,
                SslEnabled = alidayuSettings.SslEnabled,
                SandboxEnabled = alidayuSettings.SandboxEnabled,
                AppKey = alidayuSettings.AppKey,
                AppSecret = alidayuSettings.AppSecret,
                PhoneNumber = alidayuSettings.PhoneNumber,
                SmsTemplateCodeForVerificationCode = alidayuSettings.SmsTemplateCodeForVerificationCode,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.Enabled_OverrideForStore = _settingService.SettingExists(alidayuSettings, x => x.Enabled, storeScope);
                model.PhoneNumber_OverrideForStore = _settingService.SettingExists(alidayuSettings, x => x.PhoneNumber, storeScope);
            }

            return View("~/Plugins/SMS.Alidayu/Views/Configure.cshtml", model);
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
            var alidayuSettings = _settingService.LoadSetting<AlidayuSettings>(storeScope);

            //save settings
            alidayuSettings.Enabled = model.Enabled;
            alidayuSettings.ProductName = model.ProductName;
            alidayuSettings.SslEnabled = model.SslEnabled;
            alidayuSettings.SandboxEnabled = model.SandboxEnabled;
            alidayuSettings.AppKey = model.AppKey;
            alidayuSettings.AppSecret = model.AppSecret;
            alidayuSettings.PhoneNumber = model.PhoneNumber;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSetting(alidayuSettings, x => x.Enabled, storeScope, false);
            _settingService.SaveSetting(alidayuSettings, x => x.ProductName, storeScope, false);
            _settingService.SaveSetting(alidayuSettings, x => x.SslEnabled, storeScope, false);
            _settingService.SaveSetting(alidayuSettings, x => x.SandboxEnabled, storeScope, false);
            _settingService.SaveSetting(alidayuSettings, x => x.AppKey, storeScope, false);
            _settingService.SaveSetting(alidayuSettings, x => x.AppSecret, storeScope, false);
            _settingService.SaveSetting(alidayuSettings, x => x.SmsTemplateCodeForVerificationCode, storeScope, false);

            _settingService.SaveSettingOverridablePerStore(alidayuSettings, x => x.Enabled, model.Enabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(alidayuSettings, x => x.PhoneNumber, model.PhoneNumber_OverrideForStore, storeScope, false);

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

            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("Mobile.SMS.Alidayu");
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
                SuccessNotification(_localizationService.GetResource("Plugins.Sms.Alidayu.TestSuccess"));
            else
                ErrorNotification(_localizationService.GetResource("Plugins.Sms.Alidayu.TestFailed"));

            return Configure();
        }

        [HttpPost]
        //TODO
        public ActionResult SendVerificationCode(VerificationCodeModel model)
        {
            if (string.IsNullOrEmpty(model.PhoneNumber))
                throw new Exception("phone number is null or empty.");

            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var alidayuSettings = _settingService.LoadSetting<AlidayuSettings>(storeScope);

            //SMS Send VerificationCode
            if (this._verificationCodeService.SendVerificationCode(storeScope, model.PhoneNumber))
            {
                return Json(new { msg = "ok" });
            }
            else
            {
                return Json(new { msg = "" });
            }
        }

        #endregion
    }
}