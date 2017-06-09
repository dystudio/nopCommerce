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
using Nop.Plugin.SMS.Alidayu.Services;

namespace Nop.Plugin.SMS.Alidayu.Controllers
{
    public class VerificationCodeController : BasePluginController
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

        public VerificationCodeController(ILocalizationService localizationService,
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
        [HttpPost]
        [AllowAnonymous]
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