using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Plugins;
using Nop.Plugin.ExternalAuth.QQConnect;
using Nop.Plugin.ExternalAuth.QQConnect.Core;
using Nop.Plugin.ExternalAuth.QQConnect.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Nop.Plugin.ExternalAuth.QQConnect.Controllers
{
	public class ExternalAuthQQConnectController : BasePluginController
	{
		private readonly ISettingService _settingService;

		private readonly IOAuthProviderQQConnectAuthorizer _oAuthProviderQQConnectAuthorizer;

		private readonly IOpenAuthenticationService _openAuthenticationService;

		private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;

		private readonly IPermissionService _permissionService;

		private readonly IStoreContext _storeContext;

		private readonly IStoreService _storeService;

		private readonly IWorkContext _workContext;

		private readonly IPluginFinder _pluginFinder;

		private readonly ILocalizationService _localizationService;

		public ExternalAuthQQConnectController(ISettingService settingService, IOAuthProviderQQConnectAuthorizer oAuthProviderQQConnectAuthorizer, IOpenAuthenticationService openAuthenticationService, ExternalAuthenticationSettings externalAuthenticationSettings, IPermissionService permissionService, IStoreContext storeContext, IStoreService storeService, IWorkContext workContext, IPluginFinder pluginFinder, ILocalizationService localizationService)
		{
			this._settingService = settingService;
			this._oAuthProviderQQConnectAuthorizer = oAuthProviderQQConnectAuthorizer;
			this._openAuthenticationService = openAuthenticationService;
			this._externalAuthenticationSettings = externalAuthenticationSettings;
			this._permissionService = permissionService;
			this._storeContext = storeContext;
			this._storeService = storeService;
			this._workContext = workContext;
			this._pluginFinder = pluginFinder;
			this._localizationService = localizationService;
		}

		[AdminAuthorize]
		[ChildActionOnly]
		public ActionResult Configure()
		{
			ActionResult actionResult;
			if (this._permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
			{
				int storeScope = this.GetActiveStoreScopeConfiguration(this._storeService, this._workContext);
				QQConnectExternalAuthSettings qqConnectExternalAuthSettings = this._settingService.LoadSetting<QQConnectExternalAuthSettings>(storeScope);
				ConfigurationModel model = new ConfigurationModel()
				{
					ClientKeyIdentifier = qqConnectExternalAuthSettings.ClientKeyIdentifier,
					ClientSecret = qqConnectExternalAuthSettings.ClientSecret,
					ActiveStoreScopeConfiguration = storeScope,
					CallbackUrl = this._oAuthProviderQQConnectAuthorizer.GenerateLocalCallbackUri().AbsolutePath
				};
				if (storeScope > 0)
				{
					model.ClientKeyIdentifier_OverrideForStore = this._settingService.SettingExists<QQConnectExternalAuthSettings, string>(qqConnectExternalAuthSettings, (QQConnectExternalAuthSettings x) => x.ClientKeyIdentifier, storeScope);
					model.ClientSecret_OverrideForStore = this._settingService.SettingExists<QQConnectExternalAuthSettings, string>(qqConnectExternalAuthSettings, (QQConnectExternalAuthSettings x) => x.ClientSecret, storeScope);
				}
				actionResult = base.View("~/Plugins/ExternalAuth.QQConnect/Views/ExternalAuthQQConnect/Configure.cshtml", model);
			}
			else
			{
				actionResult = base.Content("Access denied");
			}
			return actionResult;
		}

		[AdminAuthorize]
		[ChildActionOnly]
		[HttpPost]
		public ActionResult Configure(ConfigurationModel model)
		{
			ActionResult actionResult;
			if (!this._permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
			{
				actionResult = base.Content("Access denied");
			}
			else if (this.ModelState.IsValid)
			{
				int storeScope = this.GetActiveStoreScopeConfiguration(this._storeService, this._workContext);
				QQConnectExternalAuthSettings qqConnectExternalAuthSettings = this._settingService.LoadSetting<QQConnectExternalAuthSettings>(storeScope);
				qqConnectExternalAuthSettings.ClientKeyIdentifier = model.ClientKeyIdentifier;
				qqConnectExternalAuthSettings.ClientSecret = model.ClientSecret;
				if ((model.ClientKeyIdentifier_OverrideForStore ? true : storeScope == 0))
				{
					this._settingService.SaveSetting<QQConnectExternalAuthSettings, string>(qqConnectExternalAuthSettings, (QQConnectExternalAuthSettings x) => x.ClientKeyIdentifier, storeScope, false);
				}
				else if (storeScope > 0)
				{
					this._settingService.DeleteSetting<QQConnectExternalAuthSettings, string>(qqConnectExternalAuthSettings, (QQConnectExternalAuthSettings x) => x.ClientKeyIdentifier, storeScope);
				}
				if ((model.ClientSecret_OverrideForStore ? true : storeScope == 0))
				{
					this._settingService.SaveSetting<QQConnectExternalAuthSettings, string>(qqConnectExternalAuthSettings, (QQConnectExternalAuthSettings x) => x.ClientSecret, storeScope, false);
				}
				else if (storeScope > 0)
				{
					this._settingService.DeleteSetting<QQConnectExternalAuthSettings, string>(qqConnectExternalAuthSettings, (QQConnectExternalAuthSettings x) => x.ClientSecret, storeScope);
				}
				this._settingService.ClearCache();
				this.SuccessNotification(this._localizationService.GetResource("Admin.Plugins.Saved"), true);
				actionResult = this.Configure();
			}
			else
			{
				actionResult = this.Configure();
			}
			return actionResult;
		}

		public ActionResult Login(string returnUrl)
		{
			return this.LoginInternal(returnUrl, false);
		}

		public ActionResult LoginCallback(string returnUrl)
		{
			return this.LoginInternal(returnUrl, true);
		}

		[NonAction]
		private ActionResult LoginInternal(string returnUrl, bool verifyResponse)
		{
            IExternalAuthenticationMethod processor = this._openAuthenticationService.LoadExternalAuthenticationMethodBySystemName("ExternalAuth.QQConnect");
            bool flag = processor == null 
                || !OpenAuthenticationExtensions.IsMethodActive(processor, this._externalAuthenticationSettings) 
                || !processor.PluginDescriptor.Installed 
                || !this._pluginFinder.AuthenticateStore(processor.PluginDescriptor, this._storeContext.CurrentStore.Id);
            if (flag)
            {
                throw new NopException("QQConnect module cannot be loaded");
            }
            LoginModel viewModel = new LoginModel();
            base.TryUpdateModel<LoginModel>(viewModel);
            AuthorizeState result = this._oAuthProviderQQConnectAuthorizer.Authorize(returnUrl, new bool?(verifyResponse));
            switch (result.AuthenticationStatus)
            {
                case OpenAuthenticationStatus.Error:
                    {
                        if (!result.Success)
                            foreach (var error in result.Errors)
                                ExternalAuthorizerHelper.AddErrorsToDisplay(error);

                        return new RedirectResult(Url.LogOn(returnUrl));
                    }
                case OpenAuthenticationStatus.AssociateOnLogon:
                    {
                        return new RedirectResult(Url.LogOn(returnUrl));
                    }
                case OpenAuthenticationStatus.AutoRegisteredEmailValidation:
                    {
                        //result
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation });
                    }
                case OpenAuthenticationStatus.AutoRegisteredAdminApproval:
                    {
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval });
                    }
                case OpenAuthenticationStatus.AutoRegisteredStandard:
                    {
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                    }
                default:
                    break;
            }
            if (result.Result != null) return result.Result;
            return HttpContext.Request.IsAuthenticated ? new RedirectResult(!string.IsNullOrEmpty(returnUrl) ? returnUrl : "~/") : new RedirectResult(Url.LogOn(returnUrl));
        }

        [ChildActionOnly]
		public ActionResult PublicInfo()
		{
			return base.View("~/Plugins/ExternalAuth.QQConnect/Views/ExternalAuthQQConnect/PublicInfo.cshtml");
		}
	}
}