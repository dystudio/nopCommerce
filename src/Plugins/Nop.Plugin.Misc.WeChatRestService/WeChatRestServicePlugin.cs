using Nop.Core.Plugins;
using Nop.Services.Common;
using System.Web.Routing;

namespace Nop.Plugin.Misc.WeChatRestService
{
    public class WeChatRestServicePlugin : BasePlugin, IMiscPlugin
    {
        #region Ctor

        public WeChatRestServicePlugin()
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
            controllerName = "WeChatAdmin";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Misc.WeChatRestService.Controllers" }, { "area", null } };
        }

        #endregion
    }
}