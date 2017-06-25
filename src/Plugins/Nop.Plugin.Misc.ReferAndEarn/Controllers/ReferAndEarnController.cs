using Microsoft.CSharp.RuntimeBinder;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Customer;
using Nop.Plugin.Misc.ReferAndEarn.Domain;
using Nop.Plugin.Misc.ReferAndEarn.Models;
using Nop.Plugin.Misc.ReferAndEarn.PluginExpiry;
using Nop.Plugin.Misc.ReferAndEarn.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.ReferAndEarn.Controllers
{
	public class ReferAndEarnController : BasePluginController
	{

		private readonly IWorkContext _workContext;

		private readonly IStoreContext _storeContext;

		private readonly IStoreService _storeService;

		private readonly ILocalizationService _localizationService;

		private readonly IWebHelper _webHelper;

		private readonly ISettingService _settingService;

		private readonly CustomerSettings _customerSettings;

		private readonly RewardPointsSettings _rewardPointsSettings;

		private readonly ForumSettings _forumSettings;

		private readonly OrderSettings _orderSettings;

		private readonly IReturnRequestService _returnRequestService;

		private readonly IReferAndEarnService _referAndEarnService;

		private readonly ICustomerService _customerService;

		private readonly HttpContextBase _httpContex;

		private readonly ICurrencyService _currencyService;

		private readonly CatalogSettings _catalogSettings;

		private readonly VendorSettings _vendorSettings;

		public ReferAndEarnController(IWorkContext workContext, IStoreContext storeContext, ILocalizationService localizationService, IWebHelper webHelper, IStoreService storeService, ISettingService settingService, CustomerSettings customerSettings, RewardPointsSettings rewardPointsSettings, ForumSettings forumSettings, OrderSettings orderSettings, IReturnRequestService returnRequestService, IReferAndEarnService referAndEarnService, ICustomerService customerService, HttpContextBase httpContex, ICurrencyService currencyService, CatalogSettings catalogSettings, VendorSettings vendorSettings)
		{
			this._workContext = workContext;
			this._storeContext = storeContext;
			this._localizationService = localizationService;
			this._webHelper = webHelper;
			this._storeService = storeService;
			this._settingService = settingService;
			this._customerSettings = customerSettings;
			this._rewardPointsSettings = rewardPointsSettings;
			this._forumSettings = forumSettings;
			this._orderSettings = orderSettings;
			this._returnRequestService = returnRequestService;
			this._referAndEarnService = referAndEarnService;
			this._customerService = customerService;
			this._httpContex = httpContex;
			this._currencyService = currencyService;
			this._catalogSettings = catalogSettings;
			this._vendorSettings = vendorSettings;
		}

		private PublicInfoModel PreparePublicInfoModel(PublicInfoModel model, Customer customer)
		{
			CustomerReferrerCode customerReferrerCodeByCustomerId = this._referAndEarnService.GetCustomerReferrerCodeByCustomerId(customer.Id);
			model.CustomerId = customer.Id;
			model.ReferrerCode = customerReferrerCodeByCustomerId.ReferrerCode;
			model.NavigationModel = this.GetCustomerNavigationModel(customer);
			return model;
		}

		[AdminAuthorize, ChildActionOnly]
		public ActionResult Configure()
		{
			int activeStoreScopeConfiguration = this.GetActiveStoreScopeConfiguration(this._storeService, this._workContext);
			ReferAndEarnSetting referAndEarnSetting = this._settingService.LoadSetting<ReferAndEarnSetting>(activeStoreScopeConfiguration);
			ConfigurationModel configurationModel = new ConfigurationModel();
			configurationModel.EnablePlugin = referAndEarnSetting.EnablePlugin;
			configurationModel.ReferrerCodeLenght = referAndEarnSetting.ReferrerCodeLenght;
			configurationModel.ReferrerRewardPoints = referAndEarnSetting.ReferrerRewardPoints;
			configurationModel.RefereeRewardPoints = referAndEarnSetting.RefereeRewardPoints;
			configurationModel.MaximumNoOfReferees = referAndEarnSetting.MaximumNoOfReferees;
			configurationModel.PurchaseLimit = referAndEarnSetting.PurchaseLimit;
			configurationModel.ReferrelRewardsForFirstPurchase = referAndEarnSetting.ReferrelRewardsForFirstPurchase;
			configurationModel.RefereeRewardsForFirstPurchase = referAndEarnSetting.RefereeRewardsForFirstPurchase;
			configurationModel.ActiveStoreScopeConfiguration = activeStoreScopeConfiguration;
			bool flag = activeStoreScopeConfiguration > 0;
			if (flag)
			{
				configurationModel.EnablePlugin_OverrideForStore = this._settingService.SettingExists<ReferAndEarnSetting, bool>(referAndEarnSetting, (ReferAndEarnSetting x) => x.EnablePlugin, activeStoreScopeConfiguration);
				configurationModel.ReferrerCodeLenght_OverrideForStore = this._settingService.SettingExists<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.ReferrerCodeLenght, activeStoreScopeConfiguration);
				configurationModel.ReferrerRewardPoints_OverrideForStore = this._settingService.SettingExists<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.ReferrerRewardPoints, activeStoreScopeConfiguration);
				configurationModel.RefereeRewardPoints_OverrideForStore = this._settingService.SettingExists<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.RefereeRewardPoints, activeStoreScopeConfiguration);
				configurationModel.MaximumNoOfReferees_OverrideForStore = this._settingService.SettingExists<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.MaximumNoOfReferees, activeStoreScopeConfiguration);
				configurationModel.ReferrelRewardsForFirstPurchase_OverrideForStore = this._settingService.SettingExists<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.ReferrelRewardsForFirstPurchase, activeStoreScopeConfiguration);
				configurationModel.PurchaseLimit_OverrideForStore = this._settingService.SettingExists<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.PurchaseLimit, activeStoreScopeConfiguration);
				configurationModel.ReferrelRewardsForFirstPurchase_OverrideForStore = this._settingService.SettingExists<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.ReferrelRewardsForFirstPurchase, activeStoreScopeConfiguration);
				configurationModel.RefereeRewardsForFirstPurchase_OverrideForStore = this._settingService.SettingExists<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.RefereeRewardsForFirstPurchase, activeStoreScopeConfiguration);
			}
			CurrencySettings currencySettings = this._settingService.LoadSetting<CurrencySettings>(activeStoreScopeConfiguration);
			configurationModel.PrimaryStoreCurrencyCode = this._currencyService.GetCurrencyById(currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
			return base.View("~/Plugins/Nop.Plugin.Misc.ReferAndEarn/Views/ReferAndEarn/Configure.cshtml", configurationModel);
		}

		[AdminAuthorize, ChildActionOnly, HttpPost]
		public ActionResult Configure(ConfigurationModel model)
		{
			int activeStoreScopeConfiguration = this.GetActiveStoreScopeConfiguration(this._storeService, this._workContext);
			ReferAndEarnSetting referAndEarnSetting = this._settingService.LoadSetting<ReferAndEarnSetting>(activeStoreScopeConfiguration);
			referAndEarnSetting.EnablePlugin = model.EnablePlugin;
			referAndEarnSetting.ReferrerCodeLenght = model.ReferrerCodeLenght;
			referAndEarnSetting.ReferrerRewardPoints = model.ReferrerRewardPoints;
			referAndEarnSetting.MaximumNoOfReferees = model.MaximumNoOfReferees;
			referAndEarnSetting.RefereeRewardPoints = model.RefereeRewardPoints;
			referAndEarnSetting.PurchaseLimit = model.PurchaseLimit;
			referAndEarnSetting.ReferrelRewardsForFirstPurchase = model.ReferrelRewardsForFirstPurchase;
			referAndEarnSetting.RefereeRewardsForFirstPurchase = model.RefereeRewardsForFirstPurchase;
			bool flag = model.EnablePlugin_OverrideForStore || activeStoreScopeConfiguration == 0;
			if (flag)
			{
				this._settingService.SaveSetting<ReferAndEarnSetting, bool>(referAndEarnSetting, (ReferAndEarnSetting x) => x.EnablePlugin, activeStoreScopeConfiguration, false);
			}
			else
			{
				bool flag2 = activeStoreScopeConfiguration > 0;
				if (flag2)
				{
					this._settingService.DeleteSetting<ReferAndEarnSetting, bool>(referAndEarnSetting, (ReferAndEarnSetting x) => x.EnablePlugin, activeStoreScopeConfiguration);
				}
			}
			bool flag3 = model.ReferrerCodeLenght_OverrideForStore || activeStoreScopeConfiguration == 0;
			if (flag3)
			{
				this._settingService.SaveSetting<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.ReferrerCodeLenght, activeStoreScopeConfiguration, false);
			}
			else
			{
				bool flag4 = activeStoreScopeConfiguration > 0;
				if (flag4)
				{
					this._settingService.DeleteSetting<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.ReferrerCodeLenght, activeStoreScopeConfiguration);
				}
			}
			bool flag5 = model.ReferrerRewardPoints_OverrideForStore || activeStoreScopeConfiguration == 0;
			if (flag5)
			{
				this._settingService.SaveSetting<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.ReferrerRewardPoints, activeStoreScopeConfiguration, false);
			}
			else
			{
				bool flag6 = activeStoreScopeConfiguration > 0;
				if (flag6)
				{
					this._settingService.DeleteSetting<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.ReferrerRewardPoints, activeStoreScopeConfiguration);
				}
			}
			bool flag7 = model.RefereeRewardPoints_OverrideForStore || activeStoreScopeConfiguration == 0;
			if (flag7)
			{
				this._settingService.SaveSetting<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.RefereeRewardPoints, activeStoreScopeConfiguration, false);
			}
			else
			{
				bool flag8 = activeStoreScopeConfiguration > 0;
				if (flag8)
				{
					this._settingService.DeleteSetting<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.RefereeRewardPoints, activeStoreScopeConfiguration);
				}
			}
			bool flag9 = model.MaximumNoOfReferees_OverrideForStore || activeStoreScopeConfiguration == 0;
			if (flag9)
			{
				this._settingService.SaveSetting<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.MaximumNoOfReferees, activeStoreScopeConfiguration, false);
			}
			else
			{
				bool flag10 = activeStoreScopeConfiguration > 0;
				if (flag10)
				{
					this._settingService.DeleteSetting<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.MaximumNoOfReferees, activeStoreScopeConfiguration);
				}
			}
			bool flag11 = model.PurchaseLimit_OverrideForStore || activeStoreScopeConfiguration == 0;
			if (flag11)
			{
				this._settingService.SaveSetting<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.PurchaseLimit, activeStoreScopeConfiguration, false);
			}
			else
			{
				bool flag12 = activeStoreScopeConfiguration > 0;
				if (flag12)
				{
					this._settingService.DeleteSetting<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.PurchaseLimit, activeStoreScopeConfiguration);
				}
			}
			bool flag13 = model.ReferrelRewardsForFirstPurchase_OverrideForStore || activeStoreScopeConfiguration == 0;
			if (flag13)
			{
				this._settingService.SaveSetting<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.ReferrelRewardsForFirstPurchase, activeStoreScopeConfiguration, false);
			}
			else
			{
				bool flag14 = activeStoreScopeConfiguration > 0;
				if (flag14)
				{
					this._settingService.DeleteSetting<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.ReferrelRewardsForFirstPurchase, activeStoreScopeConfiguration);
				}
			}
			bool flag15 = model.RefereeRewardsForFirstPurchase_OverrideForStore || activeStoreScopeConfiguration == 0;
			if (flag15)
			{
				this._settingService.SaveSetting<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.RefereeRewardsForFirstPurchase, activeStoreScopeConfiguration, false);
			}
			else
			{
				bool flag16 = activeStoreScopeConfiguration > 0;
				if (flag16)
				{
					this._settingService.DeleteSetting<ReferAndEarnSetting, int>(referAndEarnSetting, (ReferAndEarnSetting x) => x.RefereeRewardsForFirstPurchase, activeStoreScopeConfiguration);
				}
			}
			this._settingService.ClearCache();
			this.SuccessNotification(this._localizationService.GetResource("Admin.Plugins.Saved"), true);
			return this.Configure();
		}

		public ActionResult RegisterReferrerPage(string referrerCode)
		{
			bool flag = !Nop.Core.Domain.Customers.CustomerExtensions.IsRegistered(this._workContext.CurrentCustomer, true);
			ActionResult result;
			if (flag)
			{
				bool flag2 = !string.IsNullOrEmpty(referrerCode);
				if (flag2)
				{
					HttpCookie httpCookie = new HttpCookie("referrercode");
					httpCookie.Value = referrerCode;
					httpCookie.HttpOnly = true;
					httpCookie.Expires = DateTime.UtcNow.AddMonths(1);
					httpCookie.Path = "/";
					this._httpContex.Response.Cookies.Add(httpCookie);
				}
				result = base.RedirectToRoute("Register");
			}
			else
			{
				result = base.RedirectToRoute("HomePage");
			}
			return result;
		}

		public ActionResult MyAccountMenuLink()
		{
			int activeStoreScopeConfiguration = this.GetActiveStoreScopeConfiguration(this._storeService, this._workContext);
			ReferAndEarnSetting referAndEarnSetting = this._settingService.LoadSetting<ReferAndEarnSetting>(activeStoreScopeConfiguration);
			bool flag = !referAndEarnSetting.EnablePlugin;
			ActionResult result;
			if (flag)
			{
				result = base.Content("");
			}
			else
			{
				result = base.PartialView("~/Plugins/Nop.Plugin.Misc.ReferAndEarn/Views/ReferAndEarn/MyAccountMenuLink.cshtml");
			}
			return result;
		}

		public ActionResult PublicInfo()
		{
			bool flag = !Nop.Core.Domain.Customers.CustomerExtensions.IsRegistered(this._workContext.CurrentCustomer, true);
			ActionResult result;
			if (flag)
			{
				result = new HttpUnauthorizedResult();
			}
			else
			{
				base.TempData.Add("ReferAndEarnLink", "true");
				Customer currentCustomer = this._workContext.CurrentCustomer;
				PublicInfoModel publicInfoModel = new PublicInfoModel();
				publicInfoModel = this.PreparePublicInfoModel(publicInfoModel, currentCustomer);
				result = base.View("~/Plugins/Nop.Plugin.Misc.ReferAndEarn/Views/ReferAndEarn/PublicInfo.cshtml", publicInfoModel);
			}
			return result;
		}

		[HttpPost]
		public ActionResult PublicInfo(PublicInfoModel model)
		{
			Customer currentCustomer = this._workContext.CurrentCustomer;
			model = this.PreparePublicInfoModel(model, currentCustomer);
			bool isValid = base.ModelState.IsValid;
			ActionResult result;
			if (isValid)
			{
				Customer customerByEmail = this._customerService.GetCustomerByEmail(model.FriendEmail);
				bool flag = customerByEmail != null;
				if (flag)
				{
					base.ModelState.AddModelError("FriendEmail", this._localizationService.GetResource("SuperNop.Plugin.Misc.ReferAndEarn.Web.EmailExist"));
				}
				base.TempData.Add("ReferAndEarnLink", "true");
				int activeStoreScopeConfiguration = this.GetActiveStoreScopeConfiguration(this._storeService, this._workContext);
				ReferAndEarnSetting referAndEarnSetting = this._settingService.LoadSetting<ReferAndEarnSetting>(activeStoreScopeConfiguration);
				CustomerReferrerCode customerReferrerCodeByCustomerId = this._referAndEarnService.GetCustomerReferrerCodeByCustomerId(currentCustomer.Id);
				bool flag2 = customerReferrerCodeByCustomerId != null && customerReferrerCodeByCustomerId.NoOfTimesUsed >= referAndEarnSetting.MaximumNoOfReferees;
				if (flag2)
				{
					base.ModelState.AddModelError("FriendEmail", this._localizationService.GetResource("SuperNop.Plugin.Misc.ReferAndEarn.Web.MaxLimitError"));
				}
				bool flag3 = base.ModelState.IsValid && !string.IsNullOrEmpty(model.FriendEmail);
				if (flag3)
				{
					this._referAndEarnService.SendReferACustomerNotification(currentCustomer, referAndEarnSetting.ReferrerRewardPoints, model.FriendEmail, 100, this._workContext.WorkingLanguage.Id);

					this._localizationService.GetResource("SuperNop.Plugin.Misc.ReferAndEarn.Web.SucessMsg");
				}
				result = base.View("~/Plugins/Nop.Plugin.Misc.ReferAndEarn/Views/ReferAndEarn/PublicInfo.cshtml", model);
			}
			else
			{
				result = base.View("~/Plugins/Nop.Plugin.Misc.ReferAndEarn/Views/ReferAndEarn/PublicInfo.cshtml", model);
			}
			return result;
		}

		[NonAction]
		protected CustomerNavigationModel GetCustomerNavigationModel(Customer customer)
		{
			CustomerNavigationModel customerNavigationModel = new CustomerNavigationModel();
			//ICollection<CustomerNavigationItemModel> arg_49_0 = customerNavigationModel.CustomerNavigationItems;
			//CustomerNavigationItemModel expr_12 = new CustomerNavigationItemModel();
			//expr_12.RouteName("CustomerInfo");
			//expr_12.Title(this._localizationService.GetResource("Account.CustomerInfo"));
			//expr_12.Tab(0);
			//expr_12.ItemClass("customer-info");
			//arg_49_0.Add(expr_12);
			//ICollection<CustomerNavigationItemModel> arg_92_0 = customerNavigationModel.CustomerNavigationItems;
			//CustomerNavigationItemModel expr_5A = new CustomerNavigationItemModel();
			//expr_5A.set_RouteName("CustomerAddresses");
			//expr_5A.set_Title(this._localizationService.GetResource("Account.CustomerAddresses"));
			//expr_5A.set_Tab(10);
			//expr_5A.set_ItemClass("customer-addresses");
			//arg_92_0.Add(expr_5A);
			//ICollection<CustomerNavigationItemModel> arg_DB_0 = customerNavigationModel.CustomerNavigationItems;
			//CustomerNavigationItemModel expr_A3 = new CustomerNavigationItemModel();
			//expr_A3.set_RouteName("CustomerOrders");
			//expr_A3.set_Title(this._localizationService.GetResource("Account.CustomerOrders"));
			//expr_A3.set_Tab(20);
			//expr_A3.set_ItemClass("customer-orders");
			//arg_DB_0.Add(expr_A3);
			//bool flag = this._orderSettings.get_ReturnRequestsEnabled() && this._returnRequestService.SearchReturnRequests(this._storeContext.get_CurrentStore().get_Id(), this._workContext.get_CurrentCustomer().get_Id(), 0, null, null, null, null, 0, 1).Any<ReturnRequest>();
			//if (flag)
			//{
			//	ICollection<CustomerNavigationItemModel> arg_188_0 = customerNavigationModel.get_CustomerNavigationItems();
			//	CustomerNavigationItemModel expr_150 = new CustomerNavigationItemModel();
			//	expr_150.set_RouteName("CustomerReturnRequests");
			//	expr_150.set_Title(this._localizationService.GetResource("Account.CustomerReturnRequests"));
			//	expr_150.set_Tab(40);
			//	expr_150.set_ItemClass("return-requests");
			//	arg_188_0.Add(expr_150);
			//}
			//bool flag2 = !this._customerSettings.get_HideDownloadableProductsTab();
			//if (flag2)
			//{
			//	ICollection<CustomerNavigationItemModel> arg_1E7_0 = customerNavigationModel.get_CustomerNavigationItems();
			//	CustomerNavigationItemModel expr_1AF = new CustomerNavigationItemModel();
			//	expr_1AF.set_RouteName("CustomerDownloadableProducts");
			//	expr_1AF.set_Title(this._localizationService.GetResource("Account.DownloadableProducts"));
			//	expr_1AF.set_Tab(50);
			//	expr_1AF.set_ItemClass("downloadable-products");
			//	arg_1E7_0.Add(expr_1AF);
			//}
			//bool flag3 = !this._customerSettings.get_HideBackInStockSubscriptionsTab();
			//if (flag3)
			//{
			//	ICollection<CustomerNavigationItemModel> arg_246_0 = customerNavigationModel.get_CustomerNavigationItems();
			//	CustomerNavigationItemModel expr_20E = new CustomerNavigationItemModel();
			//	expr_20E.set_RouteName("CustomerBackInStockSubscriptions");
			//	expr_20E.set_Title(this._localizationService.GetResource("Account.BackInStockSubscriptions"));
			//	expr_20E.set_Tab(30);
			//	expr_20E.set_ItemClass("back-in-stock-subscriptions");
			//	arg_246_0.Add(expr_20E);
			//}
			//bool enabled = this._rewardPointsSettings.get_Enabled();
			//if (enabled)
			//{
			//	ICollection<CustomerNavigationItemModel> arg_2A2_0 = customerNavigationModel.get_CustomerNavigationItems();
			//	CustomerNavigationItemModel expr_26A = new CustomerNavigationItemModel();
			//	expr_26A.set_RouteName("CustomerRewardPoints");
			//	expr_26A.set_Title(this._localizationService.GetResource("Account.RewardPoints"));
			//	expr_26A.set_Tab(60);
			//	expr_26A.set_ItemClass("reward-points");
			//	arg_2A2_0.Add(expr_26A);
			//}
			//ICollection<CustomerNavigationItemModel> arg_2EC_0 = customerNavigationModel.get_CustomerNavigationItems();
			//CustomerNavigationItemModel expr_2B4 = new CustomerNavigationItemModel();
			//expr_2B4.set_RouteName("CustomerChangePassword");
			//expr_2B4.set_Title(this._localizationService.GetResource("Account.ChangePassword"));
			//expr_2B4.set_Tab(70);
			//expr_2B4.set_ItemClass("change-password");
			//arg_2EC_0.Add(expr_2B4);
			//bool allowCustomersToUploadAvatars = this._customerSettings.get_AllowCustomersToUploadAvatars();
			//if (allowCustomersToUploadAvatars)
			//{
			//	ICollection<CustomerNavigationItemModel> arg_347_0 = customerNavigationModel.get_CustomerNavigationItems();
			//	CustomerNavigationItemModel expr_30F = new CustomerNavigationItemModel();
			//	expr_30F.set_RouteName("CustomerAvatar");
			//	expr_30F.set_Title(this._localizationService.GetResource("Account.Avatar"));
			//	expr_30F.set_Tab(80);
			//	expr_30F.set_ItemClass("customer-avatar");
			//	arg_347_0.Add(expr_30F);
			//}
			//bool flag4 = this._forumSettings.get_ForumsEnabled() && this._forumSettings.get_AllowCustomersToManageSubscriptions();
			//if (flag4)
			//{
			//	ICollection<CustomerNavigationItemModel> arg_3B3_0 = customerNavigationModel.get_CustomerNavigationItems();
			//	CustomerNavigationItemModel expr_37B = new CustomerNavigationItemModel();
			//	expr_37B.set_RouteName("CustomerForumSubscriptions");
			//	expr_37B.set_Title(this._localizationService.GetResource("Account.ForumSubscriptions"));
			//	expr_37B.set_Tab(90);
			//	expr_37B.set_ItemClass("forum-subscriptions");
			//	arg_3B3_0.Add(expr_37B);
			//}
			//bool showProductReviewsTabOnAccountPage = this._catalogSettings.get_ShowProductReviewsTabOnAccountPage();
			//if (showProductReviewsTabOnAccountPage)
			//{
			//	ICollection<CustomerNavigationItemModel> arg_40F_0 = customerNavigationModel.get_CustomerNavigationItems();
			//	CustomerNavigationItemModel expr_3D7 = new CustomerNavigationItemModel();
			//	expr_3D7.set_RouteName("CustomerProductReviews");
			//	expr_3D7.set_Title(this._localizationService.GetResource("Account.CustomerProductReviews"));
			//	expr_3D7.set_Tab(100);
			//	expr_3D7.set_ItemClass("customer-reviews");
			//	arg_40F_0.Add(expr_3D7);
			//}
			//bool flag5 = this._vendorSettings.get_AllowVendorsToEditInfo() && this._workContext.get_CurrentVendor() != null;
			//if (flag5)
			//{
			//	ICollection<CustomerNavigationItemModel> arg_481_0 = customerNavigationModel.get_CustomerNavigationItems();
			//	CustomerNavigationItemModel expr_449 = new CustomerNavigationItemModel();
			//	expr_449.set_RouteName("CustomerVendorInfo");
			//	expr_449.set_Title(this._localizationService.GetResource("Account.VendorInfo"));
			//	expr_449.set_Tab(110);
			//	expr_449.set_ItemClass("customer-vendor-info");
			//	arg_481_0.Add(expr_449);
			//}
			return customerNavigationModel;
		}
	}
}
