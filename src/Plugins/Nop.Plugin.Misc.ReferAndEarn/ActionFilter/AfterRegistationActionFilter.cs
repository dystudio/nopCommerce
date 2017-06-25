using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using Nop.Plugin.Misc.ReferAndEarn.Services;
using System;
using System.Web;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.ReferAndEarn.ActionFilter
{
	public class AfterRegistationActionFilter : ActionFilterAttribute, IActionFilter
	{
		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			ILogger logger = EngineContext.Current.Resolve<ILogger>();
			IWorkContext workContext = EngineContext.Current.Resolve<IWorkContext>();
			IReferAndEarnService referAndEarnService = EngineContext.Current.Resolve<IReferAndEarnService>();
			HttpContextBase httpContextBase = EngineContext.Current.Resolve<HttpContextBase>();
			try
			{
				int referrerCodeAttributeId = referAndEarnService.GetReferrerCodeAttributeId();
				bool flag = referrerCodeAttributeId > 0 && filterContext.HttpContext.Request.Form.HasKeys();
				if (flag)
				{
					HttpCookie httpCookie = httpContextBase.Request.Cookies.Get("referrercode");
					bool flag2 = httpCookie != null;
					if (flag2)
					{
						referAndEarnService.AssignCodeAndRewardPoint(httpCookie.Value, filterContext.HttpContext.Request.Form["Email"]);
					}
					else
					{
						string b = string.Format("customer_attribute_{0}", referrerCodeAttributeId);
						string[] allKeys = filterContext.HttpContext.Request.Form.AllKeys;
						for (int i = 0; i < allKeys.Length; i++)
						{
							string text = allKeys[i];
							bool flag3 = text == b;
							if (flag3)
							{
								string referrerCode = filterContext.HttpContext.Request.Form[text];
								referAndEarnService.AssignCodeAndRewardPoint(referrerCode, filterContext.HttpContext.Request.Form["Email"]);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LoggingExtensions.Error(logger, ex.Message, ex, workContext.CurrentCustomer);
			}
		}
	}
}
