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
using Nop.Plugin.Misc.ReferAndEarn.Services;
using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;

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
			return base.View("~/Plugins/Misc.ReferAndEarn/Views/ReferAndEarn/Configure.cshtml", configurationModel);
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
				result = base.PartialView("~/Plugins/Misc.ReferAndEarn/Views/ReferAndEarn/MyAccountMenuLink.cshtml");
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
				result = base.View("~/Plugins/Misc.ReferAndEarn/Views/ReferAndEarn/PublicInfo.cshtml", publicInfoModel);
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
				bool alreadyExistsCustomer = customerByEmail != null;
				if (alreadyExistsCustomer)
				{
					base.ModelState.AddModelError("FriendEmail", this._localizationService.GetResource("SuperNop.Plugin.Misc.ReferAndEarn.Web.EmailExist"));
				}
				base.TempData.Add("ReferAndEarnLink", "true");
				int activeStoreScopeConfiguration = this.GetActiveStoreScopeConfiguration(this._storeService, this._workContext);
				ReferAndEarnSetting referAndEarnSetting = this._settingService.LoadSetting<ReferAndEarnSetting>(activeStoreScopeConfiguration);
				CustomerReferrerCode customerReferrerCodeByCustomerId = this._referAndEarnService.GetCustomerReferrerCodeByCustomerId(currentCustomer.Id);
				bool maximumNoOfRefereesOverflow = customerReferrerCodeByCustomerId != null && customerReferrerCodeByCustomerId.NoOfTimesUsed >= referAndEarnSetting.MaximumNoOfReferees;
				if (maximumNoOfRefereesOverflow)
				{
					base.ModelState.AddModelError("FriendEmail", this._localizationService.GetResource("SuperNop.Plugin.Misc.ReferAndEarn.Web.MaxLimitError"));
				}
				bool flag3 = base.ModelState.IsValid && !string.IsNullOrEmpty(model.FriendEmail);
				if (flag3)
				{
					this._referAndEarnService.SendReferACustomerNotification(currentCustomer, referAndEarnSetting.ReferrerRewardPoints, model.FriendEmail,referAndEarnSetting.RefereeRewardPoints, this._workContext.WorkingLanguage.Id);

					this._localizationService.GetResource("SuperNop.Plugin.Misc.ReferAndEarn.Web.SucessMsg");
				}
				result = base.View("~/Plugins/Misc.ReferAndEarn/Views/ReferAndEarn/PublicInfo.cshtml", model);
			}
			else
			{
				result = base.View("~/Plugins/Misc.ReferAndEarn/Views/ReferAndEarn/PublicInfo.cshtml", model);
			}
			return result;
		}

		[NonAction]
		protected CustomerNavigationModel GetCustomerNavigationModel(Customer customer)
		{            
            var model = new CustomerNavigationModel();

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerInfo",
                Title = _localizationService.GetResource("Account.CustomerInfo"),
                Tab = CustomerNavigationEnum.Info,
                ItemClass = "customer-info"
            });

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerAddresses",
                Title = _localizationService.GetResource("Account.CustomerAddresses"),
                Tab = CustomerNavigationEnum.Addresses,
                ItemClass = "customer-addresses"
            });

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerOrders",
                Title = _localizationService.GetResource("Account.CustomerOrders"),
                Tab = CustomerNavigationEnum.Orders,
                ItemClass = "customer-orders"
            });

            if (_orderSettings.ReturnRequestsEnabled &&
                _returnRequestService.SearchReturnRequests(_storeContext.CurrentStore.Id,
                    _workContext.CurrentCustomer.Id, pageIndex: 0, pageSize: 1).Any())
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerReturnRequests",
                    Title = _localizationService.GetResource("Account.CustomerReturnRequests"),
                    Tab = CustomerNavigationEnum.ReturnRequests,
                    ItemClass = "return-requests"
                });
            }

            if (!_customerSettings.HideDownloadableProductsTab)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerDownloadableProducts",
                    Title = _localizationService.GetResource("Account.DownloadableProducts"),
                    Tab = CustomerNavigationEnum.DownloadableProducts,
                    ItemClass = "downloadable-products"
                });
            }

            if (!_customerSettings.HideBackInStockSubscriptionsTab)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerBackInStockSubscriptions",
                    Title = _localizationService.GetResource("Account.BackInStockSubscriptions"),
                    Tab = CustomerNavigationEnum.BackInStockSubscriptions,
                    ItemClass = "back-in-stock-subscriptions"
                });
            }

            if (_rewardPointsSettings.Enabled)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerRewardPoints",
                    Title = _localizationService.GetResource("Account.RewardPoints"),
                    Tab = CustomerNavigationEnum.RewardPoints,
                    ItemClass = "reward-points"
                });
            }

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerChangePassword",
                Title = _localizationService.GetResource("Account.ChangePassword"),
                Tab = CustomerNavigationEnum.ChangePassword,
                ItemClass = "change-password"
            });

            if (_customerSettings.AllowCustomersToUploadAvatars)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerAvatar",
                    Title = _localizationService.GetResource("Account.Avatar"),
                    Tab = CustomerNavigationEnum.Avatar,
                    ItemClass = "customer-avatar"
                });
            }

            if (_forumSettings.ForumsEnabled && _forumSettings.AllowCustomersToManageSubscriptions)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerForumSubscriptions",
                    Title = _localizationService.GetResource("Account.ForumSubscriptions"),
                    Tab = CustomerNavigationEnum.ForumSubscriptions,
                    ItemClass = "forum-subscriptions"
                });
            }
            if (_catalogSettings.ShowProductReviewsTabOnAccountPage)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerProductReviews",
                    Title = _localizationService.GetResource("Account.CustomerProductReviews"),
                    Tab = CustomerNavigationEnum.ProductReviews,
                    ItemClass = "customer-reviews"
                });
            }
            if (_vendorSettings.AllowVendorsToEditInfo && _workContext.CurrentVendor != null)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerVendorInfo",
                    Title = _localizationService.GetResource("Account.VendorInfo"),
                    Tab = CustomerNavigationEnum.VendorInfo,
                    ItemClass = "customer-vendor-info"
                });
            }

            //model.SelectedTab = (CustomerNavigationEnum)selectedTabId;

            return model;
		}
	}
}
