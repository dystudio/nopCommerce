using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.Misc.WeChatRestService.Models;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.WeChatRestService.Controllers
{
    [AdminAuthorize]
    public class AdminController : BasePluginController
    {
        private readonly ISettingService _settingService;
        private readonly RestServiceSettings _settings;

        public AdminController(
            ISettingService settingService,
            RestServiceSettings settings)
        {
            _settingService = settingService;
            _settings = settings;
        }

        public ActionResult Configure()
        {
            var model = new ConfigureModel()
            {
                ApiToken = _settings.ApiToken,
                Token = _settings.Token,
                EncodingAESKey = _settings.EncodingAESKey,
                AppId = _settings.AppId,
                AppSecret=_settings.AppSecret
            };

            return View("~/Plugins/Nop.Plugin.Misc.RestService/Views/Configure.cshtml", model);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Configure(ConfigureModel model)
        {
            _settings.ApiToken = model.ApiToken;
            _settings.Token = model.Token;
            _settings.EncodingAESKey = model.EncodingAESKey;
            _settings.AppId = model.AppId;
            _settings.AppSecret = model.AppSecret;

            _settingService.SaveSetting(_settings);
            SuccessNotification("Settings saved..");

            return View("~/Plugins/Nop.Plugin.Misc.RestService/Views/Configure.cshtml", model);
        }
    }
}