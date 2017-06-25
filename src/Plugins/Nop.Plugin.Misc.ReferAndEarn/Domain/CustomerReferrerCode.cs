using Nop.Core;
using System;

namespace Nop.Plugin.Misc.ReferAndEarn.Domain
{
	public class CustomerReferrerCode : BaseEntity
	{
		public int CustomerId
		{
			get;
			set;
		}

		public string ReferrerCode
		{
			get;
			set;
		}

		public int NoOfTimesUsed
		{
			get;
			set;
		}

		public DateTime CreatedDate
		{
			get;
			set;
		}
	}
}
