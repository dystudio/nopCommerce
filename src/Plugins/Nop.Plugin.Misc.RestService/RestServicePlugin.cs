using Nop.Core.Plugins;
using Nop.Services.Common;
using System.Web.Routing;

namespace Nop.Plugin.Misc.RestService
{
    public class RestServicePlugin : BasePlugin, IMiscPlugin
    {
        #region Ctor

        public RestServicePlugin()
        {
        }

        #endregion

        #region Methods

        public override void Install()
        {
            base.Install();
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName,
            out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "Admin";
            routeValues = new RouteValueDictionary() { { "Namespaces", "panoRazzi.RestService.Controllers" }, { "area", null } };
        }

        #endregion
    }
}