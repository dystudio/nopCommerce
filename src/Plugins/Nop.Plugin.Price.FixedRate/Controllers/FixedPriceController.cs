/*
 * This file is part of 'Fixed Price provider' plug-in.

    'Fixed Price provider' plug-in is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    'Fixed Price provider' plug-in is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>
 */
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Price.FixedRate.Models;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Services.Directory;
using Nop.Plugin.Price.FixedRate.Services;
using System.Linq;
using Nop.Plugin.Price.FixedRate.Domain;
using System;
using Nop.Services.Catalog;

namespace Nop.Plugin.Price.FixedRate.Controllers
{
    [AdminAuthorize]
    public class FixedPriceController : BasePluginController
    {
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceByCountryService _priceByCountryService;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;

        public FixedPriceController(ICountryService countryService,
            ICurrencyService currencyService, IPriceByCountryService priceByCountryService,
            IProductService productService, IWorkContext workContext)
        {
            this._countryService = countryService;
            this._currencyService = currencyService;
            this._priceByCountryService = priceByCountryService;
            this._productService = productService;
            this._workContext = workContext;
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            //little hack here
            //always set culture to 'en-US' (Telerik has a bug related to editing decimal values in other cultures). Like currently it's done for admin area in Global.asax.cs
            CommonHelper.SetTelerikCulture();

            base.Initialize(requestContext);
        }


        

        public ActionResult PriceByCountry(int productId)
        {
            FixedPriceModel fixedPriceModal = new FixedPriceModel
            {
                ProductId = productId
            };

            foreach(var country in _countryService.GetAllCountries())
            {
                fixedPriceModal.AvailableCountryList.Add(new SelectListItem 
                { 
                    Text = country.Name, 
                    Value = country.Id.ToString() 
                });
            }

            foreach(var currency in _currencyService.GetAllCurrencies())
            {
                fixedPriceModal.AvailableCurrencyList.Add(new SelectListItem
                    {
                        Text = currency.CurrencyCode,
                        Value = currency.Id.ToString()
                    });
            }
            return PartialView("~/Plugins/Price.FixedRate/Views/FixedPrice/PriceByCountry.cshtml", fixedPriceModal);
        }

        [HttpPost]
        public ActionResult ProductPriceList(DataSourceRequest command, int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            var priceList = _priceByCountryService.GetPriceListByProductId(productId);
            var productPriceList = priceList
                .Select(x =>
                {
                    var productPriceModel = new FixedPriceModel.ProductPriceModel
                    {
                        Id = x.Id,
                        CountryId = x.CountryId,
                        CurrencyId = x.CurrencyId,
                        Price = x.Price,
                        Country = _countryService.GetCountryById(x.CountryId).Name,
                        Currency = _currencyService.GetCurrencyById(x.CurrencyId).CurrencyCode
                    };
                    return productPriceModel;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = productPriceList,
                Total = productPriceList.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult ProductPriceInsert(FixedPriceModel.ProductPriceModel model, int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            var priceByCountryRecord = new PriceByCountryRecord
            {
                ProductId = productId,
                CountryId = model.CountryId,
                CurrencyId = model.CurrencyId,
                Price = model.Price
            };
            _priceByCountryService.Add(priceByCountryRecord);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult ProductPriceUpdate(FixedPriceModel.ProductPriceModel model)
        {

            var priceByCountryRecord = _priceByCountryService.GetPriceByCountryRecordById(model.Id);
            if (priceByCountryRecord == null)
                throw new ArgumentException("No country specific price found for specified id");

            var product = _productService.GetProductById(priceByCountryRecord.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            priceByCountryRecord.CountryId = model.CountryId;
            priceByCountryRecord.CurrencyId = model.CurrencyId;
            priceByCountryRecord.Price = model.Price;
            _priceByCountryService.Update(priceByCountryRecord);
            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult ProductPriceDelete(int id)
        {


            var priceByCountryRecord = _priceByCountryService.GetPriceByCountryRecordById(id);
            if (priceByCountryRecord == null)
                throw new ArgumentException("No country specific price found for specified id");

            var product = _productService.GetProductById(priceByCountryRecord.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            _priceByCountryService.Delete(priceByCountryRecord);

            return new NullJsonResult();
        }

        public ActionResult Configure()
        {
            return PartialView("~/Plugins/Price.FixedRate/Views/FixedPrice/Configure.cshtml");
        }

       
    }
}
