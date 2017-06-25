using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using System;

namespace Nop.Plugin.Misc.ReferAndEarn.PluginExpiry
{
	public class PluginExpiryDependencyRegistrar : IDependencyRegistrar
	{
		public int Order
		{
			get
			{
				return 1;
			}
		}

		public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
		{
			RegistrationExtensions.RegisterType<PluginTrial>(builder).InstancePerLifetimeScope();
		}
	}
}
