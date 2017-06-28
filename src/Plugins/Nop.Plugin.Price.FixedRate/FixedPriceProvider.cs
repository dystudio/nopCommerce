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
using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Plugin.Price.FixedRate.Data;

namespace Nop.Plugin.Price.FixedRate
{
    /// <summary>
    /// Fixed Price provider
    /// </summary>
    public class FixedPriceProvider : BasePlugin
    {
        private readonly ISettingService _settingService;
        private readonly PriceByCountryRecordObjectContext _context;
        public FixedPriceProvider(ISettingService settingService, PriceByCountryRecordObjectContext context)
        {
            this._settingService = settingService;
            this._context = context;
        }
        
        
        
        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PriceFixedRate";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Price.FixedRate.Controllers" }, { "area", null } };
        }

        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Price.FixedRate.Fields.PriceCategoryName", "Price category");
            this.AddOrUpdatePluginLocaleResource("Plugins.Price.FixedRate.Fields.Rate", "Rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Price.FixedRate.Fields.PriceByCountry", "Price By Country");
            _context.Install();
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.Price.FixedRate.Fields.PriceCategoryName");
            this.DeletePluginLocaleResource("Plugins.Price.FixedRate.Fields.Rate");
            this.DeletePluginLocaleResource("Plugins.Price.FixedRate.Fields.PriceByCountry");
            _context.Uninstall();
            base.Uninstall();
        }
    }
}
