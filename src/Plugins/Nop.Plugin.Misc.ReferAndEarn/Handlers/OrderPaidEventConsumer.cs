using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Plugin.Misc.ReferAndEarn.Domain;
using Nop.Plugin.Misc.ReferAndEarn.Services;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.ReferAndEarn.Handlers
{
	public class OrderPaidEventConsumer : IConsumer<OrderPaidEvent>
	{
		private readonly IPluginFinder _pluginFinder;

		private readonly IOrderService _orderService;

		private readonly IStoreContext _storeContext;

		private readonly ISettingService _settingService;

		private readonly IReferAndEarnService _referAndEarnService;

		private readonly ICustomerService _customerService;

		private readonly IRewardPointService _rewardPointService;

		private readonly IWorkContext _workContext;

		private readonly ILocalizationService _localizationService;

		public OrderPaidEventConsumer(IPluginFinder pluginFinder, IOrderService orderService, IStoreContext storeContext, ISettingService settingService, IReferAndEarnService referAndEarnService, ICustomerService customerService, IRewardPointService rewardPointService, IWorkContext workContext, ILocalizationService localizationService)
		{
			this._pluginFinder = pluginFinder;
			this._orderService = orderService;
			this._storeContext = storeContext;
			this._settingService = settingService;
			this._referAndEarnService = referAndEarnService;
			this._customerService = customerService;
			this._rewardPointService = rewardPointService;
			this._workContext = workContext;
			this._localizationService = localizationService;
		}

		public void HandleEvent(OrderPaidEvent eventMessage)
		{
			PluginDescriptor pluginDescriptorBySystemName = this._pluginFinder.GetPluginDescriptorBySystemName("Misc.ReferAndEarn");
			if (pluginDescriptorBySystemName != null)
			{
				if (this._pluginFinder.AuthenticateStore(pluginDescriptorBySystemName, this._storeContext.CurrentStore.Id))
				{
					Order order = eventMessage.Order;
		
					if (order!= null)
					{
						Customer customer = order.Customer;
						string attribute = GenericAttributeExtensions.GetAttribute<string>(customer, "ReferrerCode", 0);
		
						if (!string.IsNullOrEmpty(attribute))
						{
							ICollection<OrderItem> orderItems = order.OrderItems;
							bool notFirstOrder = orderItems.Count > 1;
							if (!notFirstOrder)
							{
								string reffererMsg = string.Format(this._localizationService.GetResource("SuperNop.Plugin.Misc.ReferAndEarn.Order.ReffererMsg"), order.Customer.Email);
								string reffereeMsg = string.Format(this._localizationService.GetResource("SuperNop.Plugin.Misc.ReferAndEarn.Order.ReffereeMsg"), new object[0]);
								ReferAndEarnSetting referAndEarnSetting = this._settingService.LoadSetting<ReferAndEarnSetting>(this._storeContext.CurrentStore.Id);
								CustomerReferrerCode customerReferrerCodeByReferrerCodeId = this._referAndEarnService.GetCustomerReferrerCodeByReferrerCodeId(attribute);
								if (customerReferrerCodeByReferrerCodeId != null && referAndEarnSetting.PurchaseLimit <= order.OrderSubtotalExclTax)
								{
									Customer customerById = this._customerService.GetCustomerById(customerReferrerCodeByReferrerCodeId.CustomerId);
									this._rewardPointService.AddRewardPointsHistoryEntry(customerById, referAndEarnSetting.ReferrelRewardsForFirstPurchase, this._storeContext.CurrentStore.Id, reffererMsg, null, decimal.Zero, null);
									this._referAndEarnService.SendReferrerNotificationForFirstOrder(customerById, referAndEarnSetting.ReferrelRewardsForFirstPurchase, order.Customer.Email, referAndEarnSetting.RefereeRewardPoints, this._workContext.WorkingLanguage.Id);
									this._rewardPointService.AddRewardPointsHistoryEntry(order.Customer, referAndEarnSetting.RefereeRewardsForFirstPurchase, this._storeContext.CurrentStore.Id, reffereeMsg, null, decimal.Zero, null);
									this._referAndEarnService.SendRefereeNotificationForFirstOrder(order.Customer, referAndEarnSetting.RefereeRewardsForFirstPurchase, order.Customer.Email, referAndEarnSetting.RefereeRewardPoints, this._workContext.WorkingLanguage.Id);
								}
							}
						}
					}
				}
			}
		}
	}
}
