using Nop.Core.Domain.Cms;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Plugin.Misc.ReferAndEarn.Data;
using Nop.Plugin.Misc.ReferAndEarn.PluginExpiry;
using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace Nop.Plugin.Misc.ReferAndEarn
{
	public class ReferAndEarnPlugin : BasePlugin, IMiscPlugin, IPlugin, IWidgetPlugin
	{
		private readonly ILocalizationService _localizationService;

		private readonly WidgetSettings _widgetSettings;

		private readonly ISettingService _settingService;

		private readonly ReferAndEarnObjectContext _referAndEarnObjectContext;

		private readonly PluginTrial _pluginTrial;

		public ReferAndEarnPlugin(ILocalizationService localizationService, WidgetSettings widgetSettings, ISettingService settingService, ReferAndEarnObjectContext referAndEarnObjectContext, PluginTrial pluginTrial)
		{
			this._localizationService = localizationService;
			this._widgetSettings = widgetSettings;
			this._settingService = settingService;
			this._referAndEarnObjectContext = referAndEarnObjectContext;
			this._pluginTrial = pluginTrial;
		}

		public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
		{
			actionName = "Configure";
			controllerName = "ReferAndEarn";
			routeValues = new RouteValueDictionary
			{
				{
					"Namespaces",
					"Nop.Plugin.Misc.ReferAndEarn.Controllers"
				},
				{
					"area",
					null
				}
			};
		}

		public override void Install()
		{
			this._referAndEarnObjectContext.Install();
			base.Install();
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.EnablePlugin", "Enable Plugin", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrerCodeLenght", "Referrer Code Length", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrerRewardPoints", "Referrer Customer Reward Point", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.RefereeRewardPoints", "Referee Customer Reward Point", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.MaximumNoOfReferees", "Maximum Number of Referees", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.PurchaseLimit", "(First)Order value to get Reward Points", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrelRewardsForFirstPurchase", "Number of Reward Points for Referral Customer on first order", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.RefereeRewardsForFirstPurchase", "Number of Reward Points for Referee Customer on first order", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.EnablePlugin.Hint", "Select if plugin is enable", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrerCodeLenght.Hint", "Enter lenth of referrer code (Best lenght is you can set if from 8 to 12)", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrerRewardPoints.Hint", "Enter number of reward point you want to give to your referrer customer", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.RefereeRewardPoints.Hint", "Enter number of reward point you want to give to your referee customer", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.MaximumNoOfReferees.Hint", "Enter number of maximum referees per customer", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.PurchaseLimit.Hint", "Enter (First)Order value to get Reward Points", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrelRewardsForFirstPurchase.Hint", "Enter number of Reward Points for Referral Customer on first order", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.RefereeRewardsForFirstPurchase.Hint", "Enter number of Reward Points for Referee Customer on first order", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrerCodeLenght.Hint2", "(Suggested referrer code length is between 8 to 12)", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrerRewardPoints.Hint2", "(The value you have entered will be credited to referrer customer's (Customer who referrers) reward point account after successful registration of referee customer)", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.RefereeRewardPoints.Hint2", "(The value you have entered will be credited to referee customer's (New Customer who is referred) reward point account after successful registration)", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.MaximumNoOfReferees.Hint2", "(This setting will restrict those customers who will do fake registrations to get more reward points)", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.PurchaseLimit.Hint2", "(Enter minimum order amount(Exclusive Tax and Shipping) to get reward point on first order)", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrelRewardsForFirstPurchase.Hint2", "(Enter number of reward points for old/referral customer in first paid order by invited customer. Ignore or set 0 if you don't want to reward.)", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.RefereeRewardsForFirstPurchase.Hint2", "(Enter number of reward points for new/referee customer in first paid order. Ignore or set 0 if you don't want to reward.)", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Register.ReffererMsg", "Refer and Earn: Points credited for registration of your invited customer {0}", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Register.ReffereeMsg", "Refer and Earn: Points credited for your registration !!", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Order.ReffererMsg", "Refer and Earn: Points credited for first purchase of your invited customer {0}", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Order.ReffereeMsg", "Refer and Earn: Points credited for your first order !!", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Web.ReferrerCode", "Referrer Code", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Web.FriendEmailId", "Your Friend's Email", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Web.FriendEmailId.Hint2", "(Invitation email will be sent on this email with your referrer code)", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "Common.Submit", "Submit", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Web.SucessMsg", "Invitation email has been sent with referrer link.", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Web.EmailExist", "Entered email is already exist !!", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Web.MaxLimitError", "Maximum limit for your referral is over !!", null);
			LocalizationExtensions.AddOrUpdatePluginLocaleResource(this, "ReferAndEarn", "Refer and Earn", null);
			this._widgetSettings.ActiveWidgetSystemNames.Add("Nop.Plugin.Misc.ReferAndEarn");
			this._settingService.SaveSetting<WidgetSettings>(this._widgetSettings, 0);
		}

		public override void Uninstall()
		{
			this._referAndEarnObjectContext.Uninstall();
			LocalizationExtensions.DeletePluginLocaleResource(this, "Admin.Plugin.Auction.Fields.Id");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.EnablePlugin");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrerCodeLenght");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrerRewardPoints");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.RefereeRewardPoints");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.MaximumNoOfReferees");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.EnablePlugin.Hint");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrerCodeLenght.Hint");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrerRewardPoints.Hint");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.RefereeRewardPoints.Hint");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.MaximumNoOfReferees.Hint");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrerCodeLenght.Hint2");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrerRewardPoints.Hint2");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.RefereeRewardPoints.Hint2");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.MaximumNoOfReferees.Hint2");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.PurchaseLimit");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrelRewardsForFirstPurchase");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.RefereeRewardsForFirstPurchase");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.PurchaseLimit.Hint");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrelRewardsForFirstPurchase.Hint");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.RefereeRewardsForFirstPurchase.Hint");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.PurchaseLimit.Hint2");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrelRewardsForFirstPurchase.Hint2");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Fields.RefereeRewardsForFirstPurchase.Hint2");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Register.ReffererMsg");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Register.ReffereeMsg");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Order.ReffererMsg");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Order.ReffereeMsg");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Web.ReferrerCode");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Web.FriendEmailId");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Web.FriendEmailId.Hint2");
			LocalizationExtensions.DeletePluginLocaleResource(this, "Common.Submit");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Web.SucessMsg");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Web.EmailExist");
			LocalizationExtensions.DeletePluginLocaleResource(this, "SuperNop.Plugin.Misc.ReferAndEarn.Web.MaxLimitError");
			LocalizationExtensions.DeletePluginLocaleResource(this, "ReferAndEarn");
			base.Uninstall();
			this._widgetSettings.ActiveWidgetSystemNames.Remove("SuperNop.Plugin.Misc.ReferAndEarn");
			this._settingService.SaveSetting<WidgetSettings>(this._widgetSettings, 0);
			this._settingService.DeleteSetting<ReferAndEarnSetting>();
		}

		public IList<string> GetWidgetZones()
		{
			return new List<string>
			{
				"account_navigation_after"
			};
		}

		public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
		{
			actionName = "MyAccountMenuLink";
			controllerName = "ReferAndEarn";
			routeValues = new RouteValueDictionary
			{
				{
					"Namespaces",
					"Nop.Plugin.Misc.ReferAndEarn.Controllers"
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
	}
}
