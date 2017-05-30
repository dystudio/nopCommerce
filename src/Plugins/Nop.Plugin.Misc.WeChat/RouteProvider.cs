using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Misc.WeChat
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Nop.Plugin.Misc.WeChat.Index",
                 "WeChat/",
                 new { controller = "WeChat", action = "Index", },
                 new[] { "Nop.Plugin.Misc.WeChat.Controllers" }
            );
        }

        public int Priority
        {
            get { return 0; }
        }
    }
}
