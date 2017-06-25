using Nop.Core.Infrastructure;
using System;
using System.Data.Entity;

namespace Nop.Plugin.Misc.ReferAndEarn.Data
{
	public class EfStartUpTask : IStartupTask
	{
		public int Order
		{
			get
			{
				return 0;
			}
		}

		public void Execute()
		{
			Database.SetInitializer<ReferAndEarnObjectContext>(null);
		}
	}
}
