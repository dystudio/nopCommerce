using Nop.Data.Mapping;
using Nop.Plugin.Misc.ReferAndEarn.Domain;
using System;

namespace Nop.Plugin.Misc.ReferAndEarn.Data
{
	public class CustomerReferrerCodeMap : NopEntityTypeConfiguration<CustomerReferrerCode>
	{
		public CustomerReferrerCodeMap()
		{
			base.ToTable("CustomerReferrerCode");
			base.HasKey<int>((CustomerReferrerCode a) => a.Id);
		}
	}
}
