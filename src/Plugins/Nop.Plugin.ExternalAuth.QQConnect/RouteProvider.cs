using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.ExternalAuth.QQConnect
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

		public RouteProvider()
		{
		}

		public void RegisterRoutes(RouteCollection routes)
		{
			RouteCollectionExtensions.MapRoute(routes, "Plugin.ExternalAuth.QQConnect.Login", "Plugins/ExternalAuthQQConnect/Login", new { controller = "ExternalAuthQQConnect", action = "Login" }, new string[] { "Nop.Plugin.ExternalAuth.QQConnect.Controllers" });
			RouteCollectionExtensions.MapRoute(routes, "Plugin.ExternalAuth.QQConnect.LoginCallback", "Plugins/ExternalAuthQQConnect/LoginCallback", new { controller = "ExternalAuthQQConnect", action = "LoginCallback" }, new string[] { "Nop.Plugin.ExternalAuth.QQConnect.Controllers" });
		}
	}
}