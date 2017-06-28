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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Price.FixedRate.Domain
{
    public class PriceByCountryRecord:BaseEntity
    {
        public virtual int ProductId { get; set; }
        public virtual int CountryId { get; set; }
        public virtual int CurrencyId { get; set; }
        public virtual decimal Price { get; set; }
    }
}
