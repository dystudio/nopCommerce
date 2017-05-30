using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Misc.WeChatRestService
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Nop.Plugin.Misc.WeChatRestService.Index",
                 "WeChatApi/",
                 new { controller = "WeChatApi", action = "Index", },
                 new[] { "Nop.Plugin.Misc.WeChatRestService.Controllers" }
            );
        }

        public int Priority
        {
            get { return 0; }
        }
    }
}
