using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop4you.Plugin.Widgets.ThemeGrandNode.Models;
using System;
using System.Web.Mvc;

namespace Nop4you.Plugin.Widgets.ThemeGrandNode.Controllers
{
	public class ThemeGrandNodeController : BasePluginController
	{
		private readonly ISettingService _settingService;

		private readonly ThemeGrandNodeSettings _themeSettings;

		private readonly ILocalizationService _localizationService;

		public ThemeGrandNodeController(ISettingService settingService, ThemeGrandNodeSettings themeSettings, ILocalizationService localizationService)
		{
			this._settingService = settingService;
			this._themeSettings = themeSettings;
			this._localizationService = localizationService;
		}

		[AdminAuthorize, ChildActionOnly]
		public ActionResult Configure()
		{
			return base.View("~/Plugins/Widgets.ThemeGrandNode/Views/Configure.cshtml", new ConfigurationModel
			{
				Color = this._themeSettings.Color,
				showSwitchStyle = this._themeSettings.showSwitchStyle,
				showWidgetZone = this._themeSettings.WidgetZone,
				setMenuBarColor = this._themeSettings.setMenuBarColor,
				setCustomValues = this._themeSettings.setCustomValues,
				setCustomBackground = this._themeSettings.setCustomBackground,
				setCustomTextColor = this._themeSettings.setCustomTextColor,
				setCustomDarkColor = this._themeSettings.setCustomDarkColor,
				showBackToTop = this._themeSettings.showBackToTop,
				setBackToTopIcon = this._themeSettings.setBackToTopIcon,
				showBackToTopFaIcon = this._themeSettings.showBackToTopFaIcon,
				setBackToTopFaIcon = this._themeSettings.setBackToTopFaIcon,
				useStoreClosed = this._themeSettings.useStoreClosed,
				storeClosedTemplate = this._themeSettings.storeClosedTemplate,
				useCustomOrdersTemplate = this._themeSettings.useCustomOrdersTemplate,
				storeCustomOrdersTemplate = this._themeSettings.storeCustomOrdersTemplate,
				licenseKey = this._themeSettings.licenseKey,
				fixedNav = this._themeSettings.fixedNav
			});
		}

		[AdminAuthorize, ChildActionOnly, HttpPost]
		public ActionResult Configure(ConfigurationModel model)
		{
			this._themeSettings.Color = model.Color;
			this._themeSettings.showSwitchStyle = model.showSwitchStyle;
			this._themeSettings.WidgetZone = model.showWidgetZone;
			this._themeSettings.setMenuBarColor = model.setMenuBarColor;
			this._themeSettings.setCustomValues = model.setCustomValues;
			this._themeSettings.setCustomBackground = model.setCustomBackground;
			this._themeSettings.setCustomTextColor = model.setCustomTextColor;
			this._themeSettings.setCustomDarkColor = model.setCustomDarkColor;
			this._themeSettings.showBackToTop = model.showBackToTop;
			this._themeSettings.setBackToTopIcon = model.setBackToTopIcon;
			this._themeSettings.showBackToTopFaIcon = model.showBackToTopFaIcon;
			this._themeSettings.setBackToTopFaIcon = model.setBackToTopFaIcon;
			this._themeSettings.useStoreClosed = model.useStoreClosed;
			this._themeSettings.storeClosedTemplate = model.storeClosedTemplate;
			this._themeSettings.useCustomOrdersTemplate = model.useCustomOrdersTemplate;
			this._themeSettings.storeCustomOrdersTemplate = model.storeCustomOrdersTemplate;
			this._themeSettings.licenseKey = model.licenseKey;
			this._themeSettings.fixedNav = model.fixedNav;
			this._settingService.SaveSetting<ThemeGrandNodeSettings>(this._themeSettings, 0);
			this.SuccessNotification(this._localizationService.GetResource("Admin.Plugins.Saved"), true);
			return this.Configure();
		}

		[ChildActionOnly]
		public ActionResult PublicInfo(string widgetZone)
		{
			return base.View("~/Plugins/Widgets.ThemeGrandNode/Views/PublicInfo.cshtml", new ConfigurationModel
			{
				Color = this._themeSettings.Color,
				showSwitchStyle = this._themeSettings.showSwitchStyle,
				showWidgetZone = this._themeSettings.WidgetZone,
				setMenuBarColor = this._themeSettings.setMenuBarColor,
				setCustomValues = this._themeSettings.setCustomValues,
				setCustomBackground = this._themeSettings.setCustomBackground,
				setCustomTextColor = this._themeSettings.setCustomTextColor,
				setCustomDarkColor = this._themeSettings.setCustomDarkColor,
				showBackToTop = this._themeSettings.showBackToTop,
				setBackToTopIcon = this._themeSettings.setBackToTopIcon,
				showBackToTopFaIcon = this._themeSettings.showBackToTopFaIcon,
				setBackToTopFaIcon = this._themeSettings.setBackToTopFaIcon,
				useStoreClosed = this._themeSettings.useStoreClosed,
				storeClosedTemplate = this._themeSettings.storeClosedTemplate,
				useCustomOrdersTemplate = this._themeSettings.useCustomOrdersTemplate,
				storeCustomOrdersTemplate = this._themeSettings.storeCustomOrdersTemplate,
				licenseKey = this._themeSettings.licenseKey,
				fixedNav = this._themeSettings.fixedNav
			});
		}
	}
}
