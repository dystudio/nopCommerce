using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Plugins;
using Nop.Services.Localization;
using Nop.Services.Common;
using System.Web.Routing;

namespace Nop.Plugin.Misc.Captcha
{
    public class CaptchaPlugin : BasePlugin, IMiscPlugin
    {
        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "CaptchaPlugin";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Misc.CaptchaPlugin.Controllers" }, { "area", null } };
        }
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Captcha.CaptchaIsNotValid", "Wrong value of sum, please try again.");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Captcha.CaptchaIsNotValid.hint", "Wrong value of sum, please try again.");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Captcha.Captcha", "How much is the sum");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Captcha.Captcha.hint", "How much is the sum");
            base.Install();
        }
        public override void Uninstall()
        {
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Captcha.CaptchaIsNotValid");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Captcha.CaptchaIsNotValid.hint");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Captcha.Captcha");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Captcha.Captcha.hint");
            base.Uninstall();
        }
        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { "captcha_code" };
        }
    }
}
