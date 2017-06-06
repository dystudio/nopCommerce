using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using System;
using System.Web.Routing;

namespace Nop.Plugin.ExternalAuth.QQConnect
{
	public class QQConnectExternalAuthMethod : BasePlugin, IExternalAuthenticationMethod
	{
		private readonly ISettingService _settingService;

		public QQConnectExternalAuthMethod(ISettingService settingService)
		{
			this._settingService = settingService;
		}

		public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
		{
			actionName = "Configure";
			controllerName = "ExternalAuthQQConnect";
			routeValues = new RouteValueDictionary()
			{
				{ "Namespaces", "Nop.Plugin.ExternalAuth.QQConnect.Controllers" },
				{ "area", null }
			};
		}

		public void GetPublicInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
		{
			actionName = "PublicInfo";
			controllerName = "ExternalAuthQQConnect";
			routeValues = new RouteValueDictionary()
			{
				{ "Namespaces", "Nop.Plugin.ExternalAuth.QQConnect.Controllers" },
				{ "area", null }
			};
		}

		public override void Install()
		{
			QQConnectExternalAuthSettings qQConnectExternalAuthSetting = new QQConnectExternalAuthSettings()
			{
				ClientKeyIdentifier = "",
				ClientSecret = ""
			};
			this._settingService.SaveSetting<QQConnectExternalAuthSettings>(qQConnectExternalAuthSetting, 0);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.ExternalAuth.QQConnect.Login", "Login using QQConnect account", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.ExternalAuth.QQConnect.CallbackUrl", "Callback Url", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.ExternalAuth.QQConnect.ClientKeyIdentifier", "App ID/API Key", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.ExternalAuth.QQConnect.ClientKeyIdentifier.Hint", "Enter your app ID/API key here. You can find it on your QQConnect application page.", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.ExternalAuth.QQConnect.ClientSecret", "App Secret", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Plugins.ExternalAuth.QQConnect.ClientSecret.Hint", "Enter your app secret here. You can find it on your QQConnect application page.", null);
			base.Install();
		}

		public override void Uninstall()
		{
			this._settingService.DeleteSetting<QQConnectExternalAuthSettings>();
			LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.ExternalAuth.QQConnect.Login");
			LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.ExternalAuth.QQConnect.CallbackUrl");
			LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.ExternalAuth.QQConnect.ClientKeyIdentifier");
			LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.ExternalAuth.QQConnect.ClientKeyIdentifier.Hint");
			LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.ExternalAuth.QQConnect.ClientSecret");
			LocalizationExtensions.DeletePluginLocaleResource(this, "Plugins.ExternalAuth.QQConnect.ClientSecret.Hint");
			base.Uninstall();
		}
	}
}