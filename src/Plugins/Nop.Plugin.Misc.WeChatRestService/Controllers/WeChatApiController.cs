using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.WeChatRestService.Common;
using Nop.Plugin.Misc.WeChatRestService.Handlers;
using Nop.Plugin.Misc.WeChatRestService.Models;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using RestSharp;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.WxOpen.Entities.Request;
using Senparc.Weixin.MP.MvcExtension;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Nop.Plugin.Misc.WeChatRestService.DTOs;
using Nop.Services.Events;
using Nop.Services.Common;
using Nop.Plugin.Misc.WeChatRestService.Constants;

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
        private IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;

        readonly Func<string> _getRandomFileName = () => DateTime.Now.ToString("yyyyMMdd-HHmmss") + Guid.NewGuid().ToString("n").Substring(0, 6);

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
            ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService)
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
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
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
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// </summary>
        [HttpPost]
        public ActionResult Index(PostModel postModel)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, _settings.Token))
            {
                return Content("参数错误！");
            }

            postModel.Token = _settings.Token;//根据自己后台的设置保持一致
            postModel.EncodingAESKey = _settings.EncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = _settings.AppId;//根据自己后台的设置保持一致

            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
            var maxRecordCount = 10;

            var logPath = Server.MapPath(string.Format("~/App_Data/WxOpen/{0}/", DateTime.Now.ToString("yyyy-MM-dd")));
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new CustomWxOpenMessageHandler(Request.InputStream, postModel, maxRecordCount);


            try
            {
                //测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。
                messageHandler.RequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_{1}.txt", _getRandomFileName(), messageHandler.RequestMessage.FromUserName)));
                if (messageHandler.UsingEcryptMessage)
                {
                    messageHandler.EcryptRequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_Ecrypt_{1}.txt", _getRandomFileName(), messageHandler.RequestMessage.FromUserName)));
                }

                /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
                 * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
                messageHandler.OmitRepeatedMessage = true;


                //执行微信处理过程
                messageHandler.Execute();

                //测试时可开启，帮助跟踪数据

                //if (messageHandler.ResponseDocument == null)
                //{
                //    throw new Exception(messageHandler.RequestDocument.ToString());
                //}

                if (messageHandler.ResponseDocument != null)
                {
                    messageHandler.ResponseDocument.Save(Path.Combine(logPath, string.Format("{0}_Response_{1}.txt", _getRandomFileName(), messageHandler.RequestMessage.FromUserName)));
                }

                if (messageHandler.UsingEcryptMessage)
                {
                    //记录加密后的响应信息
                    messageHandler.FinalResponseDocument.Save(Path.Combine(logPath, string.Format("{0}_Response_Final_{1}.txt", _getRandomFileName(), messageHandler.RequestMessage.FromUserName)));
                }

                //return Content(messageHandler.ResponseDocument.ToString());//v0.7-
                return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
                //return new WeixinResult(messageHandler);//v0.8+
            }
            catch (Exception ex)
            {
                using (TextWriter tw = new StreamWriter(Server.MapPath("~/App_Data/Error_WxOpen_" + _getRandomFileName() + ".txt")))
                {
                    tw.WriteLine("ExecptionMessage:" + ex.Message);
                    tw.WriteLine(ex.Source);
                    tw.WriteLine(ex.StackTrace);
                    //tw.WriteLine("InnerExecptionMessage:" + ex.InnerException.Message);

                    if (messageHandler.ResponseDocument != null)
                    {
                        tw.WriteLine(messageHandler.ResponseDocument.ToString());
                    }

                    if (ex.InnerException != null)
                    {
                        tw.WriteLine("========= InnerException =========");
                        tw.WriteLine(ex.InnerException.Message);
                        tw.WriteLine(ex.InnerException.Source);
                        tw.WriteLine(ex.InnerException.StackTrace);
                    }

                    tw.Flush();
                    tw.Close();
                }
                return Content("");
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
                //return Json(new { success = true, msg = "OK", sessionId = sessionBag.Key,openId = sessionBag.OpenId, sessionKey = sessionBag.SessionKey });
                return Json(new { success = true, msg = "OK", sessionId = sessionBag.Key, openId = sessionBag.OpenId });
            }
            else
            {
                return Json(new { success = false, msg = jsonResult.errmsg });
            }
        }

        /// <summary>
        /// wx.login登陆成功之后发送的请求成功之后回调此函数，用来获取客户信息或者创建客户
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetOrCreateCustomer(UserInfoDto userInfo)
        {
            if (string.IsNullOrEmpty(userInfo.OpenId))
                return Json(new { success = false, msg = "openId could not be empty" });

            var currentCustomer = _customerService.GetCustomerBySystemName(userInfo.OpenId);

            if (currentCustomer == null)
            {
                currentCustomer = _customerService.InsertGuestCustomer();
                currentCustomer.RegisteredInStoreId = _storeContext.CurrentStore.Id;
                bool isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
                currentCustomer.Active = isApproved;
                currentCustomer.Username = userInfo.OpenId;
                currentCustomer.CustomerGuid = Guid.NewGuid();
                currentCustomer.SystemName = userInfo.OpenId;
                currentCustomer.IsSystemAccount = false;

                if (_customerSettings.GenderEnabled) {
                    string gender = string.Empty;
                    switch (userInfo.Gender)
                    {
                        case "1":
                            gender = "M";
                            break;
                        case "2":
                            gender = "F";
                            break;
                        default:
                            gender = "M";
                            break;
                    }
                    _genericAttributeService.SaveAttribute(currentCustomer, SystemCustomerAttributeNames.Gender, gender);
                }
                if (_customerSettings.CityEnabled)
                    _genericAttributeService.SaveAttribute(currentCustomer, SystemCustomerAttributeNames.City, userInfo.City);
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    _genericAttributeService.SaveAttribute(currentCustomer, SystemCustomerAttributeNames.StateProvinceId,userInfo.Province);

                //for wechat specific property
                _genericAttributeService.SaveAttribute(currentCustomer, WeChatCustomerAttributeNames.OpenId, userInfo.OpenId);
                _genericAttributeService.SaveAttribute(currentCustomer, WeChatCustomerAttributeNames.NickName, userInfo.NickName);
                _genericAttributeService.SaveAttribute(currentCustomer, WeChatCustomerAttributeNames.Gender, userInfo.Gender);
                _genericAttributeService.SaveAttribute(currentCustomer, WeChatCustomerAttributeNames.City, userInfo.City);
                _genericAttributeService.SaveAttribute(currentCustomer, WeChatCustomerAttributeNames.Province, userInfo.Province);
                _genericAttributeService.SaveAttribute(currentCustomer, WeChatCustomerAttributeNames.AvatarUrl, userInfo.AvatarUrl);
                _genericAttributeService.SaveAttribute(currentCustomer, WeChatCustomerAttributeNames.UnionId, userInfo.UnionId);


                _customerService.UpdateCustomer(currentCustomer);
                //raise event       
                _eventPublisher.Publish(new CustomerRegisteredEvent(currentCustomer));
            }
            
            return Json(new { success = true, Customer = new { currentCustomer.Id,currentCustomer.SystemName,currentCustomer.Username}, msg = "OK" });

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