using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Web.Controllers;
using Nop.Plugin.Misc.ReferAndEarn.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.ReferAndEarn.ActionFilter
{
	public class ReferAndEarnFilterProvider : IFilterProvider
	{
		private readonly IPluginFinder _pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();

		private readonly IWorkContext _workContext = EngineContext.Current.Resolve<IWorkContext>();

		private readonly IReferAndEarnService _referAndEarnService = EngineContext.Current.Resolve<IReferAndEarnService>();

		private readonly IStoreContext _storeContext = EngineContext.Current.Resolve<IStoreContext>();

		private readonly ISettingService _settingService = EngineContext.Current.Resolve<ISettingService>();

		private readonly ILogger _logger = EngineContext.Current.Resolve<ILogger>();

		//private readonly IActionFilter _actionFilterBeforeRegistration = new BeforeRegistationActionFilter();

		private readonly IActionFilter _actionFilterAfterRegistration = new AfterRegistationActionFilter();

		private string RandomString(int length)
		{
			Random random = new Random();
			return new string((from s in Enumerable.Repeat<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789", length)
			select s[random.Next(s.Length)]).ToArray<char>());
		}

		public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
		{
			bool flag = actionDescriptor.ControllerDescriptor.ControllerType == typeof(CustomerController) && actionDescriptor.ActionName.Equals("Register") && controllerContext.HttpContext.Request.HttpMethod == "GET" && this._pluginFinder.GetPluginDescriptorBySystemName("Nop.Plugin.Misc.ReferAndEarn") != null;
			IEnumerable<Filter> result;
			if (flag)
			{
				result = new Filter[]
				{
					//new Filter(this._actionFilterBeforeRegistration, 30, new int?(101))
				};
			}
			else
			{
				bool flag2 = actionDescriptor.ControllerDescriptor.ControllerType == typeof(CustomerController) && actionDescriptor.ActionName.Equals("Register") && controllerContext.HttpContext.Request.HttpMethod == "POST" && this._pluginFinder.GetPluginDescriptorBySystemName("Nop.Plugin.Misc.ReferAndEarn") != null;
				if (flag2)
				{
					result = new Filter[]
					{
						new Filter(this._actionFilterAfterRegistration,new FilterScope(),new int?(101))
					};
				}
				else
				{
					result = new Filter[0];
				}
			}
			return result;
		}
	}
}
