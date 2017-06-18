using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace Nop4you.Plugin.Widgets.ThemeGrandNode
{
	public class ThemeGrandNodePlugin : BasePlugin, IWidgetPlugin, IPlugin
	{
		private readonly ISettingService _settingService;

		private readonly ThemeGrandNodeSettings _themeSettings;

		public ThemeGrandNodePlugin(ISettingService settingService, ThemeGrandNodeSettings themeSettings)
		{
			this._settingService = settingService;
			this._themeSettings = themeSettings;
		}

		public IList<string> GetWidgetZones()
		{
			return new List<string>
			{
				string.IsNullOrEmpty(this._themeSettings.WidgetZone) ? "switchstylesheet" : this._themeSettings.WidgetZone
			};
		}

		public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
		{
			actionName = "Configure";
			controllerName = "ThemeGrandNode";
			routeValues = new RouteValueDictionary
			{
				{
					"Namespaces",
					"Nop4you.Plugin.Widgets.ThemeGrandNode.Controllers"
				},
				{
					"area",
					null
				}
			};
		}

		public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
		{
			actionName = "PublicInfo";
			controllerName = "ThemeGrandNode";
			routeValues = new RouteValueDictionary
			{
				{
					"Namespaces",
					"Nop4you.Plugin.Widgets.ThemeGrandNode.Controllers"
				},
				{
					"area",
					null
				},
				{
					"widgetZone",
					widgetZone
				}
			};
		}

		public override void Install()
		{
			ThemeGrandNodeSettings settings = new ThemeGrandNodeSettings
			{
				WidgetZone = "switchstylesheet",
				Color = "green",
				setBackToTopIcon = "navigation",
				setBackToTopFaIcon = "fa-caret-up"
			};
			this._settingService.SaveSetting<ThemeGrandNodeSettings>(settings, 0);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.Color", "Color", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.Color.hint", "Choose color template", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.showSwitchStyle", "Show switch style", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.showSwitchStyle.hint", "Check if style switch should be vissible.", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.showWidgetZone", "Widget zone", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.showWidgetZone.hint", "Choose widget zone for style switch.", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.setMenuBarColor", "Override menu bar color", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.setMenuBarColor.hint", "Check if menu bar color should be in color of the theme", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.setCustomValues", "Own color style", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.setCustomValues.hint", "Check if you want to use own colors", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.setCustomBackground", "Background", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.setCustomBackground.hint", "Choose color for background", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.setCustomTextColor", "Text", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.setCustomTextColor.hint", "Choose color for text", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.setCustomDarkColor", "Hover color effect", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.setCustomDarkColor.hint", "Choose color for hover effect", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.showBackToTop", "Show back to top", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.showBackToTop.hint", "Check if back to top button should be activated.", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.setBackToTopIcon", "Set back to top icon", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.setBackToTopIcon.hint", "Set back to top icon from materialize icon list", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.showBackToTopfa", "Use Font Awesome Icon", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.showBackToTopfa.hint", "Check if you want to use Font Awesome Icon instead of Materialize Icon", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.setBackToTopFaIcon", "Font Awesome Icon", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.setBackToTopFaIcon.hint", "Choose Font Awesome Icon for back to top button", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.configurationTabColors", "Colors", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.configurationTabStoreClosed", "Store closed page", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.widgetConfiguration", "GrandNode Theme installer", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.useStoreClosed", "Use Store Closed", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.useStoreClosed.hint", "Choose to use one of predefined Store Closed Pages", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.storeClosedTemplate", "Store Closed Template", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.storeClosedTemplate.hint", "Choose Store Closed pages template", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.showColorsBar", "Show colors bar", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.useCustomOrdersTemplate", "Check to use custom order template", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.useCustomOrdersTemplate.hint", "Check to use custom order template with predifined colours", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.storeCustomOrdersTemplate", "Choose template", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.storeCustomOrdersTemplate.hint", "Choose order template view", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.configurationCustomOrders", "Customer Orders Templates", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.licenseKey", "License Key", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.Monday", "Monday", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.Tuesday", "Tuesday", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.Wednesday", "Wednesday", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.Thursday", "Thursday", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.Friday", "Friday", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.Saturday", "Saturday", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.Sunday", "Sunday", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.fixedNav", "Fixed navigation", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.fixedNav.hint", "Check to made navigation bar fixed to top while scrolling down", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Nop4you.Theme.GrandNode.configurationTabSettings", "Settings", null);
			base.Install();
		}

		public override void Uninstall()
		{
			base.Uninstall();
		}
	}
}
