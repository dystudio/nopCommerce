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
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.WeChatRestService.Controllers
{
    public class WeChatApiController : BaseController
    {
        #region Fields

        private ICustomerService _customerService;
        private IOrderService _orderService;
        private IWorkContext _workContext;
        private IVendorService _vendorService;
        private IProductAttributeParser _productAttributeParser;
        private WeChatRestServiceSettings _settings;
        private CustomerSettings _customerSettings;
        private ICustomerRegistrationService _customerRegistrationService;
        private IStoreContext _storeContext;
        private IProductService _productService;
        private ICategoryService _categoryService;
        private ICacheManager _cacheManager;
        #endregion

        #region Ctor

        public WeChatApiController(
            ICustomerService customerService,
            IOrderService orderService,
            IWorkContext workContext,
            IProductAttributeParser productAttributeParser,
            IVendorService vendorService,
            WeChatRestServiceSettings settings,
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

        #region WeChat small app api
        /// <summary>
        /// GET请求用于处理微信小程序后台的URL验证
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(PostModel postModel, string echostr)
        {
            var token = _settings.Token;
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信小程序后台的Url，请注意保持Token一致。");
            }
        }

        /// <summary>
        /// wx.login登陆成功之后发送的请求
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnLogin(string code)
        {
            var jsonResult = SnsApi.JsCode2Json(_settings.AppId, _settings.AppSecret, code);
            if (jsonResult.errcode == ReturnCode.请求成功)
            {
                //Session["WxOpenUser"] = jsonResult;//使用Session保存登陆信息（不推荐）
                //使用SessionContainer管理登录信息（推荐）
                var sessionBag = SessionContainer.UpdateSession(null, jsonResult.openid, jsonResult.session_key);

                //注意：生产环境下SessionKey属于敏感信息，不能进行传输！
                return Json(new { success = true, msg = "OK", sessionId = sessionBag.Key, sessionKey = sessionBag.SessionKey });
            }
            else
            {
                return Json(new { success = false, msg = jsonResult.errmsg });
            }
        }

        [HttpPost]
        public ActionResult CheckWxOpenSignature(string sessionId, string rawData, string signature)
        {
            try
            {
                var checkSuccess = Senparc.Weixin.WxOpen.Helpers.EncryptHelper.CheckSignature(sessionId, rawData, signature);
                return Json(new { success = checkSuccess, msg = checkSuccess ? "签名校验成功" : "签名校验失败" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.Message });
            }
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