using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Web.Framework.Mvc;
using Nop.Plugin.Misc.ReferAndEarn.ActionFilter;
using Nop.Plugin.Misc.ReferAndEarn.Data;
using Nop.Plugin.Misc.ReferAndEarn.Domain;
using Nop.Plugin.Misc.ReferAndEarn.Services;
using System;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.ReferAndEarn
{
	public class DependencyRegistrar : IDependencyRegistrar
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
            Autofac.RegistrationExtensions.RegisterType<ReferAndEarnService>(builder).As<IReferAndEarnService>().InstancePerLifetimeScope();
			DependencyRegistrarExtensions.RegisterPluginDataContext<ReferAndEarnObjectContext>(this, builder, "nop_object_context_sncreferandearn_module");
            Autofac.RegistrationExtensions.WithParameter<EfRepository<CustomerReferrerCode>, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Autofac.RegistrationExtensions.RegisterType<EfRepository<CustomerReferrerCode>>(builder).As<IRepository<CustomerReferrerCode>>(), ResolvedParameter.ForNamed<IDbContext>("nop_object_context_sncreferandearn_module")).InstancePerLifetimeScope();
            Autofac.RegistrationExtensions.RegisterType<ReferAndEarnFilterProvider>(builder).As<IFilterProvider>().InstancePerLifetimeScope();
		}
	}
}
