using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.WeChatRestService.Common;
using Nop.Plugin.Misc.WeChatRestService.Models;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.WeChatRestService.Controllers
{
    public class ApiController : BaseController
    {
        #region Fields

        private ICustomerService _customerService;
        private IOrderService _orderService;
        private IWorkContext _workContext;
        private IVendorService _vendorService;
        private IProductAttributeParser _productAttributeParser;
        private RestServiceSettings _settings;
        private CustomerSettings _customerSettings;
        private ICustomerRegistrationService _customerRegistrationService;
        private IStoreContext _storeContext;
        private IProductService _productService;
        private ICategoryService _categoryService;
        private ICacheManager _cacheManager;
        #endregion

        #region Ctor

        public ApiController(
            ICustomerService customerService,
            IOrderService orderService,
            IWorkContext workContext,
            IProductAttributeParser productAttributeParser,
            IVendorService vendorService,
            RestServiceSettings settings,
            CustomerSettings customerSettings,
            ICustomerRegistrationService customerRegistrationService,
            IStoreContext storeContext,
            IProductService productService,
            ICategoryService categoryService,
            ICacheManager cacheManager)
        {
            _customerService = customerService;
            _orderService = orderService;
            _workContext = workContext;
            _productAttributeParser = productAttributeParser;
            _vendorService = vendorService;
            _settings = settings;
            _customerSettings = customerSettings;
            _customerRegistrationService = customerRegistrationService;
            _storeContext = storeContext;
            _productService = productService;
            _categoryService = categoryService;
            _cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
        }

        #endregion

        #region Access Token
        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetAccessToken(string clientId, string clientSecret, string serverUrl, string redirectUrl, string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            var state = Guid.NewGuid();

            var client = new RestClient(serverUrl);
            var request = new RestRequest("/oauth/authorize", Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", clientId); // adds to POST or URL querystring based on Method
            request.AddParameter("redirect_uri", redirectUrl);
            request.AddParameter("response_type", "code");
            request.AddParameter("state", state);

            var cachedUserAccessModel = _cacheManager.Get(state.ToString(), () =>
            {
                return new UserAccessModel
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    RedirectUrl = redirectUrl,
                    ServerUrl = serverUrl
                };
            });
            IRestResponse response = client.Execute(request);
            var accessTokenViewModel = JsonConvert.DeserializeObject<AccessTokenViewModel>(response.Content); // raw content as json string
            return Json(accessTokenViewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetToken(string code, string state)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
                return ErrorOccured("code or state is empty.");
            var userAccessModel = _cacheManager.Get(state, () =>
            {
                return new UserAccessModel();
            });
            var authParameters = _cacheManager.Get(code + state, () =>
            {
                return new AuthParameters()
                {
                    ClientId = userAccessModel.ClientId,
                    ClientSecret = userAccessModel.ClientSecret,
                    ServerUrl = userAccessModel.ServerUrl,
                    RedirectUrl = userAccessModel.RedirectUrl,
                    GrantType = "authorization_code",
                    Code = code
                };
            });
            var nopAuthorizationManager = new AuthorizationManager(authParameters.ClientId, authParameters.ClientSecret, authParameters.ServerUrl);
            string responseJson = nopAuthorizationManager.GetAuthorizationData(authParameters);
            AuthorizationModel authorizationModel = JsonConvert.DeserializeObject<AuthorizationModel>(responseJson);
            return Json(authorizationModel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Misc

        public ActionResult InvalidApiToken(string apiToken)
        {
            var errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(apiToken))
                errorMessage = "No API token supplied.";
            else
                errorMessage = string.Format("Invalid API token: {0}", apiToken);

            return ErrorOccured(errorMessage);
        }

        public ActionResult ErrorOccured(string errorMessage)
        {
            return Json(new
            {
                success = false,
                errorMessage = errorMessage
            });
        }

        public ActionResult Successful(object data)
        {
            return Json(new
            {
                success = true,
                data = data
            });
        }

        #endregion

        #region Helper methods

        private bool IsApiTokenValid(string apiToken)
        {
            if (string.IsNullOrWhiteSpace(apiToken) ||
                string.IsNullOrWhiteSpace(_settings.ApiToken))
                return false;

            return _settings.ApiToken.Trim().Equals(apiToken.Trim(),
                StringComparison.InvariantCultureIgnoreCase);
        }
      
      
        
        #endregion
    }
}