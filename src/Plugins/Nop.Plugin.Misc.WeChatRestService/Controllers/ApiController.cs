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

        #region Customers

        /// <summary>
        /// To Get Customer Details
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <returns></returns>
        public ActionResult GetCustomerById(int id, string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
                return ErrorOccured("Customer not found.");


            return Json(GetCustomerJson(customer), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// To Get All Customers
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllCustomer(string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            var customers = _customerService.GetAllCustomers();

            return Json(GetCustomersJson(customers), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Customer by USername
        /// </summary>
        /// <param name="username">Email of Customer</param>
        /// <param name="apiToken"></param>
        /// <returns></returns>
        public ActionResult GetCustomerByUsername(string username, string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            var customer = _customerService.GetCustomerByUsername(username);
            if (customer == null)
                return ErrorOccured("Customer not found.");

            return Successful(GetCustomerJson(customer));
        }

        /// <summary>
        /// Get Customer By Email
        /// </summary>
        /// <param name="email">Email of Customer</param>
        /// <param name="apiToken"></param>
        /// <returns></returns>
        public ActionResult GetCustomerByEmail(string email, string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            if (String.IsNullOrWhiteSpace(email))
                return ErrorOccured("Email is empty.");

            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                return ErrorOccured("Customer not found.");

            return Successful(GetCustomerJson(customer));
        }

        public ActionResult GetCustomerByGuid(string guid, string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            if (String.IsNullOrWhiteSpace(guid))
                return ErrorOccured("Guid is empty.");

            Guid id = new Guid(guid);

            var customer = _customerService.GetCustomerByGuid(id);
            if (customer == null)
                return ErrorOccured("Customer not found.");

            return Successful(GetCustomerJson(customer));
        }

        public ActionResult GetCustomerBySystemName(string systemName, string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            if (String.IsNullOrWhiteSpace(systemName))
                return ErrorOccured("System Name is empty.");

            var customer = _customerService.GetCustomerBySystemName(systemName);
            if (customer == null)
                return ErrorOccured("Customer not found.");

            return Successful(GetCustomerJson(customer));
        }

        public ActionResult GetAllCustomers(string apiToken, DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null, int affiliateId = 0, int vendorId = 0,
            string customerRoleIds = null, string email = null, string username = null,
            string firstName = null, string lastName = null,
            int dayOfBirth = 0, int monthOfBirth = 0,
            string company = null, string phone = null, string zipPostalCode = null,
            string loadOnlyWithShoppingCart = null, int shoppingCartTypeId = 0,
            int pageIndex = 0, int pageSize = 2147483647)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            int[] customerRole = null;
            ShoppingCartType? sct = null;
            bool isLoadOnlyWithShoppingCart = false;

            if (!String.IsNullOrWhiteSpace(customerRoleIds))
            {
                customerRole = Array.ConvertAll(customerRoleIds.Split(','), int.Parse);
            }

            if (shoppingCartTypeId > 0)
            {
                sct = (ShoppingCartType)shoppingCartTypeId;
            }

            if (!String.IsNullOrEmpty(loadOnlyWithShoppingCart))
            {
                if (loadOnlyWithShoppingCart.ToLower().Equals("true"))
                {
                    isLoadOnlyWithShoppingCart = true;
                }
            }

            var customers = _customerService.GetAllCustomers(createdFromUtc, createdToUtc, affiliateId, vendorId,
                customerRole, email, username, firstName, lastName, dayOfBirth, monthOfBirth, company, phone,
                zipPostalCode, string.Empty, isLoadOnlyWithShoppingCart, sct, pageIndex, pageSize);

            if (customers.Count == 0)
                return ErrorOccured("Customers are not found.");

            return Successful(GetCustomersJson(customers));
        }

        public ActionResult GetOnlineCustomers(string apiToken, string customerRoleIds = null,
             int pageIndex = 0, int pageSize = 2147483647)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            int[] customerRole = null;

            if (!String.IsNullOrWhiteSpace(customerRoleIds))
            {
                customerRole = Array.ConvertAll(customerRoleIds.Split(','), int.Parse);
            }

            var customers = _customerService.GetOnlineCustomers(DateTime.UtcNow.AddMinutes(-_customerSettings.OnlineCustomerMinutes), customerRole, pageIndex, pageSize);

            if (customers.Count == 0)
                return ErrorOccured("Customers are not found.");

            return Successful(GetCustomersJson(customers));
        }

        public ActionResult GetCustomersByIds(string apiToken, string customerRoleIds = null)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            int[] customerRole = null;

            if (!String.IsNullOrWhiteSpace(customerRoleIds))
            {
                customerRole = Array.ConvertAll(customerRoleIds.Split(','), int.Parse);
            }

            var customers = _customerService.GetCustomersByIds(customerRole);

            if (customers.Count == 0)
                return ErrorOccured("Customers are not found.");

            return Successful(GetCustomersJson(customers));
        }
        #endregion

        #region Product

        /// <summary>
        /// Get Products for the Homepage
        /// </summary>
        /// <param name="pageNum">Page Number w.r.t Calls</param>
        /// <param name="pageSize">Number of Products to be returned on each call</param>
        /// <param name="categoryId">Category Id of the Products</param>
        /// <param name="searchString">String to be Searched</param>
        /// <param name="minimumPriceValue">Minimum Value for the Product</param>
        /// <param name="maximumPriceValue">Maximum Value for the Product</param>
        /// <param name="color">Color of the Product</param>
        /// <param name="brand"></param>
        /// <param name="ratings"></param>
        /// <returns>List of Products Matching Specific Criteria</returns>

        /// <summary>
        /// Get Detail of a Single Product
        /// </summary>
        /// <param name="productID"></param>
        /// <returns>Detail of Product</returns>
        public ActionResult GetProductsDetails(int productID, string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);
            Product product = _productService.GetProductById(productID);
            return Json(new { Product = product }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Sign In Sign Up
        ///api/login?Username={admin@yourstore.com}&Password={ideofuzion}
        /// <summary>
        /// login
        /// </summary>
        /// <param name="Email">Customer Email or username</param>
        /// <param name="Password">Cutomer Password</param>
        /// <returns></returns>
        public ActionResult Login(Customer customer, string password, string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);
            APIResponse apiResponse = new APIResponse();
            var loginResult = _customerRegistrationService.ValidateCustomer(customer.Email, password);
            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                    apiResponse.data = CustomerLoginResults.Successful;
                    apiResponse.StatusCode = 200;
                    break;
                case CustomerLoginResults.CustomerNotExist:
                    apiResponse.data = CustomerLoginResults.CustomerNotExist;
                    apiResponse.StatusCode = 200;
                    break;
                case CustomerLoginResults.WrongPassword:
                    apiResponse.data = CustomerLoginResults.WrongPassword;
                    apiResponse.StatusCode = 200;
                    break;
                case CustomerLoginResults.Deleted:
                    apiResponse.data = CustomerLoginResults.Deleted;
                    apiResponse.StatusCode = 200;
                    break;
                case CustomerLoginResults.NotActive:
                    apiResponse.data = CustomerLoginResults.NotActive;
                    apiResponse.StatusCode = 200;
                    break;
                case CustomerLoginResults.NotRegistered:
                    apiResponse.data = CustomerLoginResults.NotRegistered;
                    apiResponse.StatusCode = 200;
                    break;
            }
            return Json(new { data = apiResponse }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Registers the Customer by its email
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>JSON Result</returns>
        [HttpPost]
        public ActionResult Register(Customer receivedCustomer, string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            APIResponse apiResponse = new APIResponse();
            bool isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
            var customer = _workContext.CurrentCustomer;
            var registrationRequest = new CustomerRegistrationRequest(customer,
                    receivedCustomer.Email,
                    receivedCustomer.Email,
                    //receivedCustomer.Password,
                    "123456",
                    _customerSettings.DefaultPasswordFormat,
                    _storeContext.CurrentStore.Id,
                    isApproved);
            var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
            if (registrationResult.Success)
            {
                apiResponse.StatusCode = 200;
                apiResponse.data = _customerService.GetCustomerByEmail(receivedCustomer.Email);
            }
            else
            {
                apiResponse.StatusCode = 400;
                apiResponse.data = null;
            }
            return Json(new { data = apiResponse }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Categoies
        /// <summary>
        /// Gets the Categories
        /// </summary>
        /// <returns>List of Stored Categories in DB</returns>
        public ActionResult getAllCategories(string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);
            List<Category> categoryList = _categoryService.GetAllCategories().ToList();
            return Json(new { CategoryList = categoryList }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Orders

        public ActionResult GetOrderById(int id, string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            var order = _orderService.GetOrderById(id);
            if (order == null)
                return ErrorOccured("Order not found.");

            return Successful(GetOrderJson(order));
        }


        public ActionResult GetOrderByGuid(Guid orderGuid, string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            var order = _orderService.GetOrderByGuid(orderGuid);
            if (order == null)
                return ErrorOccured("Order not found");

            return Successful(GetOrderJson(order));
        }


        public ActionResult GetOrderByAuthorizationTransactionIdAndPaymentMethod(
            string authorizationTransactionId, string paymentMethodSystemName,
            string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            var order = _orderService.GetOrderByAuthorizationTransactionIdAndPaymentMethod(
                authorizationTransactionId, paymentMethodSystemName);
            if (order == null)
                return ErrorOccured("Order not found");

            return Successful(GetOrderJson(order));
        }

        public ActionResult GetOrderItemByGuid(Guid orderItemGuid, string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            var orderItem = _orderService.GetOrderItemByGuid(orderItemGuid);
            if (orderItem == null)
                return ErrorOccured("Order item not founf");

            return Successful(GetOrderItemJson(orderItem));
        }

        public ActionResult GetOrderItemById(int orderItemId, string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            var orderItem = _orderService.GetOrderItemById(orderItemId);

            if (orderItem == null)
                return ErrorOccured("Order item not found");

            return Successful(GetOrderItemJson(orderItem));
        }

        public ActionResult GetOrderNoteById(int id, string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return InvalidApiToken(apiToken);

            var orderNote = _orderService.GetOrderNoteById(id);
            if (orderNote == null)
                return ErrorOccured("Ordernote not found");

            return Successful(GetOrderNoteJson(orderNote));
        }

        // TODO: add paging
        public ActionResult GetOrdersByVendorId(int vendorId)
        {
            //if (!IsApiTokenValid(apiToken))
            //    return InvalidApiToken(apiToken);

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                return ErrorOccured("Vendor not found.");

            var vendorOrders = _orderService.SearchOrders(vendorId: vendorId);
            return Json(GetOrdersJson(vendorOrders, vendorId), JsonRequestBehavior.AllowGet);
            //return Successful(GetOrdersJson(vendorOrders, vendorId));
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

        private object GetAddressJson(Address address)
        {
            if (address == null)
                return null;

            var addressJson = new
            {
                FirstName = address.FirstName,
                LastName = address.LastName,
                Email = address.Email,
                Company = address.Company,
                CountryId = address.CountryId,
                CountryName = address.Country == null ? null : address.Country.Name,
                StateProvinceId = address.StateProvinceId,
                StateProvinceName = address.StateProvince == null ? null : address.StateProvince.Name,
                City = address.City,
                Address1 = address.Address1,
                Address2 = address.Address2,
                ZipPostalCode = address.ZipPostalCode,
                PhoneNumber = address.PhoneNumber,
                FaxNumber = address.FaxNumber,
                CreatedOnUtc = address.CreatedOnUtc
            };

            return addressJson;
        }

        private object GetCustomerJson(Customer customer)
        {
            // TODO: refactor into own method for reuse
            var customerRoles = customer.CustomerRoles
                .Select(c =>
                    new
                    {
                        Id = c.Id,
                        Name = c.Name,
                        SystemName = c.SystemName
                    });

            // TODO: refactor into own method for reuse
            var externalAuthenticationRecords = customer.ExternalAuthenticationRecords
                .Select(e =>
                    new
                    {
                        Id = e.Id,
                        CustomerId = e.CustomerId,
                        Email = e.Email,
                        ExternalIdentifier = e.ExternalIdentifier,
                        ExternalDisplayIdentifier = e.ExternalDisplayIdentifier,
                        OAuthToken = e.OAuthToken,
                        OAuthAccessToken = e.OAuthAccessToken,
                        ProviderSystemName = e.ProviderSystemName
                    });

            // TODO: refactor into own method for reuse
            var shoppingCartItem = customer.ShoppingCartItems
                .Select(c =>
                    new
                    {
                        Id = c.Id,
                        StoreId = c.StoreId,
                        ShoppingCartTypeId = c.ShoppingCartTypeId,
                        CustomerId = c.CustomerId,
                        ProductId = c.ProductId,
                        AttributesXml = c.AttributesXml,
                        CustomerEnteredPrice = c.CustomerEnteredPrice,
                        Quantity = c.Quantity,
                        CreatedOnUtc = c.CreatedOnUtc,
                        UpdatedOnUtc = c.UpdatedOnUtc,
                        IsFreeShipping = c.IsFreeShipping,
                        IsShipEnabled = c.IsShipEnabled,
                        AdditionalShippingCharge = c.AdditionalShippingCharge,
                        IsTaxExempt = c.IsTaxExempt
                    });

            var customerJson = new
            {
                Id = customer.Id,
                CustomerGuid = customer.CustomerGuid,
                UserName = customer.Username,
                Email = customer.Email,
                CustomerRoles = customerRoles,
                AdminComment = customer.AdminComment,
                IsTaxExempt = customer.IsTaxExempt,
                AffiliateId = customer.AffiliateId,
                VendorId = customer.VendorId,
                HasShoppingCartItems = customer.HasShoppingCartItems,
                Active = customer.Active,
                Deleted = customer.Deleted,
                IsSystemAccount = customer.IsSystemAccount,
                SystemName = customer.SystemName,
                LastIpAddress = customer.LastIpAddress,
                CreatedOnUtc = customer.CreatedOnUtc,
                LastLoginDateUtc = customer.LastLoginDateUtc,
                LastActivityDateUtc = customer.LastActivityDateUtc,
                ExternalAuthenticationRecords = externalAuthenticationRecords,
                ShoppingCartItems = shoppingCartItem
            };

            return customerJson;
        }

        // TODO: add in paging info
        private object GetCustomersJson(IList<Customer> customers)
        {
            var customerJsonList = new List<object>();

            foreach (var customer in customers)
            {
                customerJsonList.Add(GetCustomerJson(customer));
            }

            return customerJsonList;
        }

        private object GetOrderJson(Order order, int vendorId = 0)
        {
            IEnumerable<OrderItem> orderItems = order.OrderItems;
            if (vendorId > 0)
                orderItems = orderItems.Where(item => item.Product.VendorId == vendorId);

            var orderJson = new
            {
                Id = order.Id,
                Customer = GetCustomerJson(order.Customer),
                BillingAddress = GetAddressJson(order.BillingAddress),
                ShippingAddress = GetAddressJson(order.ShippingAddress),
                OrderStatusId = order.OrderStatusId,
                OrderStatus = order.OrderStatus.ToString(),
                ShippingStatusId = order.ShippingStatusId,
                ShippingStatus = order.ShippingStatus.ToString(),
                PaymentStatusId = order.PaymentStatusId,
                PaymentStatus = order.PaymentStatus.ToString(),
                OrderItems = orderItems.Select(item => GetOrderItemJson(item)),
                // need to add in other properties to complete this
            };

            return orderJson;
        }

        // TODO: add in paging info
        private object GetOrdersJson(IPagedList<Order> orders, int vendorId = 0)
        {
            if (orders == null)
                return null;

            var ordersJson = orders.Select(o => GetOrderJson(o, vendorId));

            return ordersJson;
        }

        private object GetOrderItemJson(OrderItem orderItem)
        {
            var orderItemJson = new
            {
                AttributeDescription = orderItem.AttributeDescription,
                AttributesXml = orderItem.AttributesXml,
                DiscountAmountExclTax = orderItem.DiscountAmountExclTax,
                DiscountAmountInclTax = orderItem.DiscountAmountInclTax,
                DownloadCount = orderItem.DownloadCount,
                IsDownloadActivated = orderItem.IsDownloadActivated,
                ItemWeight = orderItem.ItemWeight,
                LicenseDownloadId = orderItem.LicenseDownloadId,
                //Order 
                OrderId = orderItem.OrderId,
                OrderItemGuid = orderItem.OrderItemGuid,
                OriginalProductCost = orderItem.OriginalProductCost,
                PriceExclTax = orderItem.PriceExclTax,
                PriceInclTax = orderItem.PriceInclTax,
                // Product 
                ProductId = orderItem.ProductId,
                ProductName = orderItem.Product.Name,
                Quantity = orderItem.Quantity,
                UnitPriceExclTax = orderItem.UnitPriceExclTax,
                UnitPriceInclTax = orderItem.UnitPriceInclTax,
            };

            return orderItemJson;
        }

        private object GetOrderNoteJson(OrderNote orderNote)
        {
            var orderNoteJson = new
            {
                CreatedOnUtc = orderNote.CreatedOnUtc,
                DisplayToCustomer = orderNote.DisplayToCustomer,
                DownloadId = orderNote.DownloadId,
                Note = orderNote.Note,
                OrderId = orderNote.OrderId,
            };

            return orderNoteJson;
        }

        #endregion
    }
}