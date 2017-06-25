//using Nop.Core;
//using Nop.Core.Domain.Customers;
//using Nop.Core.Infrastructure;
//using Nop.Services.Customers;
//using Nop.Services.Logging;
//using Nop.Web.Models.Customer;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Web;
//using System.Web.Mvc;

//namespace Nop.Plugin.Misc.ReferAndEarn.ActionFilter
//{
//	public class BeforeRegistationActionFilter : ActionFilterAttribute, IActionFilter
//	{
//		[CompilerGenerated]
//		[Serializable]
//		private sealed class <>c
//		{
//			public static readonly BeforeRegistationActionFilter.<>c <>9 = new BeforeRegistationActionFilter.<>c();

//			public static Func<CustomerAttributeModel, bool> <>9__0_0;

//			internal bool <OnActionExecuted>b__0_0(CustomerAttributeModel c)
//			{
//				return c.get_Name() == "Referrer Code";
//			}
//		}

//		public override void OnActionExecuted(ActionExecutedContext filterContext)
//		{
//			ILogger logger = EngineContext.get_Current().Resolve<ILogger>();
//			IWorkContext workContext = EngineContext.get_Current().Resolve<IWorkContext>();
//			try
//			{
//				RegisterModel registerModel = filterContext.get_Controller().get_ViewData().get_Model() as RegisterModel;
//				HttpCookie httpCookie = filterContext.get_RequestContext().HttpContext.Request.Cookies.Get("referrercode");
//				bool flag = httpCookie != null && registerModel != null;
//				if (flag)
//				{
//					IEnumerable<CustomerAttributeModel> arg_84_0 = registerModel.get_CustomerAttributes();
//					Func<CustomerAttributeModel, bool> arg_84_1;
//					if ((arg_84_1 = BeforeRegistationActionFilter.<>c.<>9__0_0) == null)
//					{
//						arg_84_1 = (BeforeRegistationActionFilter.<>c.<>9__0_0 = new Func<CustomerAttributeModel, bool>(BeforeRegistationActionFilter.<>c.<>9.<OnActionExecuted>b__0_0));
//					}
//					arg_84_0.Where(arg_84_1).FirstOrDefault<CustomerAttributeModel>().set_DefaultValue(httpCookie.Value);
//				}
//				ICustomerService customerService = EngineContext.get_Current().Resolve<ICustomerService>();
//				Customer currentCustomer = workContext.get_CurrentCustomer();
//				currentCustomer.set_Deleted(true);
//				currentCustomer.set_Active(false);
//				workContext.set_CurrentCustomer(customerService.InsertGuestCustomer());
//			}
//			catch (Exception ex)
//			{
//				LoggingExtensions.Error(logger, ex.Message, ex, workContext.get_CurrentCustomer());
//			}
//		}
//	}
//}
