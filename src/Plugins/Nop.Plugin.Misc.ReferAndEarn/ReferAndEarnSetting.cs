using Nop.Core.Configuration;
using System;

namespace Nop.Plugin.Misc.ReferAndEarn
{
	public class ReferAndEarnSetting : ISettings
	{
		public bool EnablePlugin
		{
			get;
			set;
		}

		public int ReferrerCodeLenght
		{
			get;
			set;
		}

		public int ReferrerRewardPoints
		{
			get;
			set;
		}

		public int RefereeRewardPoints
		{
			get;
			set;
		}

		public int MaximumNoOfReferees
		{
			get;
			set;
		}

		public int ReferrelRewardsForFirstPurchase
		{
			get;
			set;
		}

		public int RefereeRewardsForFirstPurchase
		{
			get;
			set;
		}

		public int PurchaseLimit
		{
			get;
			set;
		}
	}
}
