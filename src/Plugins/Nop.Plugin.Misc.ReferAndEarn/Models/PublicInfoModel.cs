using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Customer;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.ReferAndEarn.Models
{
	public class PublicInfoModel : BaseNopModel
	{
		public CustomerNavigationModel NavigationModel
		{
			get;
			set;
		}

		public int CustomerId
		{
			get;
			set;
		}

		[NopResourceDisplayName("SuperNop.Plugin.Misc.ReferAndEarn.Web.ReferrerCode")]
		public string ReferrerCode
		{
			get;
			set;
		}

		[NopResourceDisplayName("SuperNop.Plugin.Misc.ReferAndEarn.Web.FriendEmailId"), EmailAddress, Required]
		public string FriendEmail
		{
			get;
			set;
		}
	}
}
