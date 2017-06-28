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
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Plugin.Price.FixedRate.Domain;
using Nop.Services.Directory;
using Nop.Web.Models.ShoppingCart;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Price.FixedRate.Services
{
    public class PriceByCountryService : IPriceByCountryService
    {
        private readonly IRepository<PriceByCountryRecord> _PriceByCountryRecordRepository;
        private ICacheManager _cacheManager;
        private readonly IGeoLookupService _geoLookupService;      
        private const string PRODUCTPRICE_ALL_KEY = "Nop.productprice.all-{0}";

        public PriceByCountryService(
            IRepository<PriceByCountryRecord> PriceByCountryRecordRepository, ICacheManager cacheManager,
            IGeoLookupService geoLookupService)
        {
            this._PriceByCountryRecordRepository = PriceByCountryRecordRepository;
            this._cacheManager = cacheManager;
            this._geoLookupService = geoLookupService;
        }

        public PriceByCountryService(IRepository<PriceByCountryRecord> PriceByCountryRecordRepository)
        {
            _PriceByCountryRecordRepository = PriceByCountryRecordRepository;
        }

        public virtual void Add(PriceByCountryRecord record)
        {
            _PriceByCountryRecordRepository.Insert(record);
        }

        public virtual void Update(PriceByCountryRecord record)
        {
            _PriceByCountryRecordRepository.Update(record);
        }

        public virtual void Delete(Domain.PriceByCountryRecord record)
        {
            _PriceByCountryRecordRepository.Delete(record);
        }

        public virtual IList<Domain.PriceByCountryRecord> GetPriceListByProductId(int productId)
        {
            string key = string.Format(PRODUCTPRICE_ALL_KEY, productId);

            return _cacheManager.Get(key, () =>
            {
                var query = from productprice in _PriceByCountryRecordRepository.Table
                            orderby productprice.Id
                            where productprice.ProductId == productId
                            select productprice;
                var productPriceList = query.ToList();
                return productPriceList;
            });
        }

        public virtual PriceByCountryRecord GetProductPriceByCountryId(int countryId, int productId)
        {
            var query = from productprice in _PriceByCountryRecordRepository.Table
                        where productprice.CountryId == countryId && productprice.ProductId == productId
                        select productprice;
            var productPriceRecord = query.SingleOrDefault();
            return productPriceRecord;
        }

        public virtual PriceByCountryRecord GetPriceByCountryRecordById(int id)
        {
            var query = from productprice in _PriceByCountryRecordRepository.Table
                        orderby productprice.Id
                        where productprice.Id == id
                        select productprice;
            var productPriceRecord = query.SingleOrDefault();
            return productPriceRecord;
        }

        public virtual int GetCountryId()
        {
            var countryService = EngineContext.Current.Resolve<ICountryService>();
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            string ipAddress;
            ipAddress = webHelper.GetCurrentIpAddress();
            string isoCode = _geoLookupService.LookupCountryIsoCode(ipAddress);
            if (!string.IsNullOrEmpty(isoCode))
            {
                var country = countryService.GetCountryByTwoLetterIsoCode(isoCode);
                return country.Id;
            }
            else
            {
                return 0;
            }
        }
        public virtual string GetCountrySpecificPrice(int countryId, int productId)
        {
            var currencyService = EngineContext.Current.Resolve<ICurrencyService>();
            string price = string.Empty;
            var priceByCountryRecord = GetProductPriceByCountryId(countryId, productId);
            if (priceByCountryRecord != null)
            {
                var currency = currencyService.GetCurrencyById(priceByCountryRecord.CurrencyId);
                price = priceByCountryRecord.Price.ToString(currency.CustomFormatting);
            }
            return price;
        }

        public virtual string GetSubTotal(int countryId, int quantity, int productId)
        {
            var currencyService = EngineContext.Current.Resolve<ICurrencyService>();            
            string subTotal = string.Empty;
            var priceByCountryRecord = GetProductPriceByCountryId(countryId, productId);
            if (priceByCountryRecord != null)
            {
                var currency = currencyService.GetCurrencyById(priceByCountryRecord.CurrencyId);
                subTotal = (quantity * priceByCountryRecord.Price).ToString(currency.CustomFormatting);
            }
            return subTotal;
        }
public virtual OrderTotalsModel GetOrderTotal(OrderTotalsModel orderTotalModel, int countryId)
        {
            var customer = EngineContext.Current.Resolve<IWorkContext>().CurrentCustomer;
            var cartItems = customer.ShoppingCartItems;
            decimal orderTotal = 0, subTotal = 0;
            int i = 0, currencyId = 0;
            foreach(var cartItem in cartItems)
            {
                if(i == 0)
                {
                    currencyId = GetCurrencyId(countryId, cartItem.ProductId);
                    if(currencyId > 0)
                        i++;
                }
                subTotal = subTotal + (cartItem.Quantity * GetProductPrice(countryId, cartItem.ProductId));
            }

            orderTotalModel.SubTotal = FormatPrice(subTotal, currencyId);
            orderTotalModel.OrderTotal = FormatPrice(subTotal, currencyId);
            return orderTotalModel;
        }

        public virtual decimal GetProductPrice(int countryId, int productId)
        {
            var priceByCountryRecord = GetProductPriceByCountryId(countryId, productId);
            return priceByCountryRecord.Price;
        }
        public virtual string FormatPrice(decimal price, int currencyId)
        {
            var currencyService = EngineContext.Current.Resolve<ICurrencyService>();
            var currency = currencyService.GetCurrencyById(currencyId);
            return price.ToString(currency.CustomFormatting);
        }

        public virtual int GetCurrencyId(int countryId, int productId)
        {
            var currencyService = EngineContext.Current.Resolve<ICurrencyService>();
            
            var priceByCountryRecord = GetProductPriceByCountryId(countryId, productId);
            if (priceByCountryRecord != null)
                return priceByCountryRecord.CurrencyId;
            else
                return 0;
        }
    }
}
