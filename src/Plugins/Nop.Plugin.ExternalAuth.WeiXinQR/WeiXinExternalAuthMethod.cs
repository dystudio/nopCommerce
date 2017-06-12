using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Localization;
using Nop.Services.Configuration;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.WeiXinQR
{
    public class WeiXinExternalAuthMethod : BasePlugin, IExternalAuthenticationMethod
    {
        private readonly ISettingService _settingService;

        public WeiXinExternalAuthMethod(ISettingService settingService)
        {
            this._settingService = settingService;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ExternalAuthWeiXinQR";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.ExternalAuth.WeiXinQR.Controllers" }, { "area", null } };
        }

        public void GetPublicInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "ExternalAuthWeiXinQR";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.ExternalAuth.WeiXinQR.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// 安装插件
        /// </summary>
        public override void Install()
        {
            // settings
            var settings = new WeiXinQRExternalAuthSettings()
            {
                AppId = "",
                AppSecret = ""
            };
            _settingService.SaveSetting(settings);

            // locales
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeiXinQR.Login", "微信登录");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeiXinQR.AppId", "唯一凭证");
            this.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.WeiXinQR.AppSecret", "唯一凭证密钥");

            base.Install();
        }

        /// <summary>
        /// 卸载插件
        /// </summary>
        public override void Uninstall()
        {
            // settings
            _settingService.DeleteSetting<WeiXinQRExternalAuthSettings>();

            this.DeletePluginLocaleResource("Plugins.ExternalAuth.WeiXinQR.Login");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.WeiXinQR.AppId");
            this.DeletePluginLocaleResource("Plugins.ExternalAuth.WeiXinQR.AppSecret");
            this.DeletePluginLocaleResource("Plugins.FriendlyName.ExternalAuth.WeiXinQR");

            base.Uninstall();
        }
    }
}
