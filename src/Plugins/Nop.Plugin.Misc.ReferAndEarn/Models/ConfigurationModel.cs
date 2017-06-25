using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.ReferAndEarn.Models
{
	public class ConfigurationModel : BaseNopModel
	{
		public int ActiveStoreScopeConfiguration
		{
			get;
			set;
		}

		[NopResourceDisplayName("SuperNop.Plugin.Misc.ReferAndEarn.Fields.EnablePlugin"), Required]
		public bool EnablePlugin
		{
			get;
			set;
		}

		public bool EnablePlugin_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrerCodeLenght")]
		public int ReferrerCodeLenght
		{
			get;
			set;
		}

		public bool ReferrerCodeLenght_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrerRewardPoints")]
		public int ReferrerRewardPoints
		{
			get;
			set;
		}

		public bool ReferrerRewardPoints_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("SuperNop.Plugin.Misc.ReferAndEarn.Fields.RefereeRewardPoints")]
		public int RefereeRewardPoints
		{
			get;
			set;
		}

		public bool RefereeRewardPoints_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("SuperNop.Plugin.Misc.ReferAndEarn.Fields.MaximumNoOfReferees")]
		public int MaximumNoOfReferees
		{
			get;
			set;
		}

		public bool MaximumNoOfReferees_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("SuperNop.Plugin.Misc.ReferAndEarn.Fields.ReferrelRewardsForFirstPurchase")]
		public int ReferrelRewardsForFirstPurchase
		{
			get;
			set;
		}

		public bool ReferrelRewardsForFirstPurchase_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("SuperNop.Plugin.Misc.ReferAndEarn.Fields.RefereeRewardsForFirstPurchase")]
		public int RefereeRewardsForFirstPurchase
		{
			get;
			set;
		}

		public bool RefereeRewardsForFirstPurchase_OverrideForStore
		{
			get;
			set;
		}

		[NopResourceDisplayName("SuperNop.Plugin.Misc.ReferAndEarn.Fields.PurchaseLimit")]
		public int PurchaseLimit
		{
			get;
			set;
		}

		public bool PurchaseLimit_OverrideForStore
		{
			get;
			set;
		}

		public string PrimaryStoreCurrencyCode
		{
			get;
			set;
		}
	}
}
