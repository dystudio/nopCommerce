using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Misc.Captcha.Infrastructure
{
   public class RouteProvider:IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapLocalizedRoute("MyHomePagePlugin",
                            "Nop.Plugin.Misc.Captcha/CaptchaPlugin/Index",
                            new { controller = "CaptchaPlugin", action = "Index" },
                            new[] { "Nop.Plugin.Misc.Captcha.Controllers" });


        }

        public int Priority
        {
            get
            {
                return int.MaxValue; ;
            } 
        }
    }
}
