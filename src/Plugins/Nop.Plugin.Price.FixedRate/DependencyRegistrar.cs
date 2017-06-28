/*
 * This file is part of 'Fixed Price provider' plug-in.

    'Fixed Price provider' plug-in is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    'Fixed Price provider' plug-in is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>
 */
using Autofac;
using Autofac.Core;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Price.FixedRate.Data;
using Nop.Plugin.Price.FixedRate.Domain;
using Nop.Plugin.Price.FixedRate.Services;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Price.FixedRate
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_product_price_by_country";
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<FixedPriceByCountry>().As<System.Web.Mvc.IFilterProvider>();
            builder.RegisterType<PriceByCountryService>().As<IPriceByCountryService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<PriceByCountryRecordObjectContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<PriceByCountryRecord>>()
                .As<IRepository<PriceByCountryRecord>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 1; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, Core.Configuration.NopConfig config)
        {
            builder.RegisterType<FixedPriceByCountry>().As<System.Web.Mvc.IFilterProvider>();
            builder.RegisterType<PriceByCountryService>().As<IPriceByCountryService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<PriceByCountryRecordObjectContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<PriceByCountryRecord>>()
                .As<IRepository<PriceByCountryRecord>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
        }
    }
}
