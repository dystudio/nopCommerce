using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.JobScheduler
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.JobScheduler.SchedulerList",
                "Plugins/JobSchedulerAdmin/SchedulerList",
                new { controller = "JobScheduler", action = "List" },
                new[] { "Nop.Plugin.JobScheduler.Controllers" });

            routes.MapRoute("Plugin.JobScheduler.NewCustomerList",
              "Plugins/JobSchedulerAdmin/NewCustomerList",
              new { controller = "VisitCustomer", action = "NewCustomer" },
              new[] { "Nop.Plugin.JobScheduler.Controllers" });

            routes.MapRoute("Plugin.JobScheduler.RunSchecdulerNow",
                "Plugins/JobSchedulerAdmin/RunSchecdulerNow/{Id}",
                 new { controller = "JobScheduler", action = "RunSchecdulerNow" },
                new[] { "Nop.Plugin.JobScheduler.Controllers" });
        }

        public int Priority
        {
            get { return 0; }
        }
    }
}
