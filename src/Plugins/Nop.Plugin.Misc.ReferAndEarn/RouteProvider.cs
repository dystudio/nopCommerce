using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Misc.ReferAndEarn
{
	public class RouteProvider : IRouteProvider
	{
		public int Priority
		{
			get
			{
				return 0;
			}
		}

		public void RegisterRoutes(RouteCollection routes)
		{
			RouteCollectionExtensions.MapRoute(routes, "SuperNop.Plugin.Misc.ReferAndEarn.Configure", "Plugin/ReferAndEarn/Configure/", new
			{
				controller = "ReferAndEarn",
				action = "Configure"
			}, new string[]
			{
				"Nop.Plugin.Misc.ReferAndEarn.Controllers"
			});
			RouteCollectionExtensions.MapRoute(routes, "SuperNop.Plugin.Misc.ReferAndEarn.ReferAndEarn", "ReferAndEarn/PublicInfo/", new
			{
				controller = "ReferAndEarn",
				action = "PublicInfo"
			}, new string[]
			{
				"Nop.Plugin.Misc.ReferAndEarn.Controllers"
			});
			RouteCollectionExtensions.MapRoute(routes, "SuperNop.Plugin.Misc.ReferAndEarn.RegisterReferral", "RegisterReferral/", new
			{
				controller = "ReferAndEarn",
				action = "RegisterReferrerPage"
			}, new string[]
			{
				"Nop.Plugin.Misc.ReferAndEarn.Controllers"
			});
		}
	}
}
