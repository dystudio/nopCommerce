using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.ExternalAuth.WeiXin
{
    public class RouteProvider : IRouteProvider
    {

        public void RegisterRoutes(RouteCollection routes)
        {
            // Login
            routes.MapRoute("Plugin.ExternalAuth.WeiXinQR.Login",
                "Plugins/ExternalAuthWeiXinQR/Login",
                new { controller = "Nop.Plugin.ExternalAuth.WeiXinQR.Controllers", action = "Login" },
                new[] { "Nop.Plugin.ExternalAuth.WeiXin.Controllers" });

            // LoginCallback
            routes.MapRoute("Plugin.ExternalAutn.WeiXinQR.LoginCallback",
                "Plugins/ExternalAuthWeiXinQR/LoginCallback",
                new { controller = "Nop.Plugin.ExternalAuth.WeiXinQR.Controllers", action = "LoginCallback" },
                new[] { "Nop.Plugin.ExternalAuth.WeiXinQR.Controllers" });
        }

        public int Priority
        {
            get { return 0; }
        }
    }
}
