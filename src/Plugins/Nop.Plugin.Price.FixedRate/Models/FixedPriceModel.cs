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
using Nop.Core.Domain.Directory;
using Nop.Plugin.Price.FixedRate.Domain;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Plugin.Price.FixedRate.Models
{
    public class FixedPriceModel
    {
        public int ProductId { get; set; }
        public IList<SelectListItem> AvailableCountryList { get; set; }
        public IList<SelectListItem> AvailableCurrencyList { get; set; }     
   
        public FixedPriceModel()     
        {           
            AvailableCountryList = new List<SelectListItem>();
            AvailableCurrencyList = new List<SelectListItem>();
        }

        public partial class ProductPriceModel : BaseNopEntityModel
        {
            public override int Id { get; set; }
            public string Country { get; set; }
            public int CountryId { get; set; }
            public string Currency { get; set; }
            public int CurrencyId { get; set; }
            public decimal Price { get; set; }
        }
    }

    
}