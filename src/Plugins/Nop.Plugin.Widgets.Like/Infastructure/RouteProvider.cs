using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Nop.Web.Framework.Localization;
namespace Nop.Plugin.Widgets.Like.Infastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
           
            routes.MapLocalizedRoute("Widgets.Like.CustomerProductLike",
                          "Widgets.Like.Like/{productId}",
                          new { controller = "WidgetsLike", action = "ProductLike" },
                           new { productId = @"\d+" },
                          new[] { "Nop.Web.Controllers" });
            routes.MapLocalizedRoute("Widgets.Like.CustomerProductUnLike",
                            "Widgets.Like.Unlike/{productId}",
                           new { controller = "WidgetsLike", action = "ProductUnLike" },
                            new { productId = @"\d+" },
                           new[] { "Nop.Web.Controllers" });
            routes.MapLocalizedRoute("Widgets.Like.LikelistForCustomer",
                            "Like",
                           new { controller = "WidgetsLike", action = "LikeListForCustomer" },
                           new[] { "Nop.Web.Controllers" });

        }
        public int Priority
        {
            get
            {
                return 2;
            }
        }
    }
}
