using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Plugin.Misc.ReferAndEarn.PluginExpiry
{
	public class LicenseKeyModel : BaseNopModel
	{
		public string LicenseKey
		{
			get;
			set;
		}

		public string ExpiryDate
		{
			get;
			set;
		}
	}
}
