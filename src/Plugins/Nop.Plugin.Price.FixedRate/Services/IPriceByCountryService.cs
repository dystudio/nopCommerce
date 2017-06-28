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
using Nop.Plugin.Price.FixedRate.Domain;
using Nop.Web.Models.ShoppingCart;
using System.Collections.Generic;

namespace Nop.Plugin.Price.FixedRate.Services
{
    public interface IPriceByCountryService
    {
        void Add(PriceByCountryRecord record);
        void Update(PriceByCountryRecord record);
        void Delete(PriceByCountryRecord record);
        IList<PriceByCountryRecord> GetPriceListByProductId(int productId);
        PriceByCountryRecord GetProductPriceByCountryId(int countryId, int productId);
        PriceByCountryRecord GetPriceByCountryRecordById(int id);
        string GetCountrySpecificPrice(int countryId, int productId);
        string GetSubTotal(int countryId, int quantity, int productId);
        OrderTotalsModel GetOrderTotal(OrderTotalsModel orderTotalModel, int countryId);
        decimal GetProductPrice(int countryId, int productId);
        string FormatPrice(decimal price, int currencyId);
        int GetCurrencyId(int countryId, int productId);
        int GetCountryId();
    }
}
