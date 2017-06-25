//using System;
//using System.Globalization;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Routing;

//namespace Nop.Plugin.Misc.ReferAndEarn.PluginExpiry
//{
//	public class CheckLicenseAttribute : ActionFilterAttribute
//	{
//		private readonly bool _ignore;

//		public CheckLicenseAttribute(bool ignore = false)
//		{
//			this._ignore = ignore;
//		}

//		public override void OnActionExecuting(ActionExecutingContext filterContext)
//		{
//			bool flag = filterContext == null || filterContext.get_HttpContext() == null;
//			if (!flag)
//			{
//				bool ignore = this._ignore;
//				if (!ignore)
//				{
//					HttpRequestBase request = filterContext.get_HttpContext().Request;
//					bool flag2 = request == null;
//					if (!flag2)
//					{
//						string actionName = filterContext.get_ActionDescriptor().get_ActionName();
//						bool flag3 = string.IsNullOrEmpty(actionName);
//						if (!flag3)
//						{
//							string value = filterContext.get_Controller().ToString();
//							bool flag4 = string.IsNullOrEmpty(value);
//							if (!flag4)
//							{
//								bool isChildAction = filterContext.get_IsChildAction();
//								if (!isChildAction)
//								{
//									PluginTrial pluginTrial = new PluginTrial();
//									string expiryDate = pluginTrial.GetExpiryDate();
//									bool flag5 = !string.IsNullOrEmpty(expiryDate) && !string.IsNullOrWhiteSpace(expiryDate);
//									bool flag7;
//									if (flag5)
//									{
//										DateTime t = DateTime.ParseExact(expiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
//										bool flag6 = t >= DateTime.UtcNow;
//										if (flag6)
//										{
//											return;
//										}
//										flag7 = true;
//									}
//									else
//									{
//										flag7 = true;
//									}
//									bool flag8 = flag7;
//									if (flag8)
//									{
//										filterContext.set_Result(new RedirectToRouteResult(new RouteValueDictionary
//										{
//											{
//												"action",
//												"PluginExpiry"
//											},
//											{
//												"controller",
//												"ReferAndEarnPluginExpiry"
//											}
//										}));
//									}
//								}
//							}
//						}
//					}
//				}
//			}
//		}
//	}
//}
