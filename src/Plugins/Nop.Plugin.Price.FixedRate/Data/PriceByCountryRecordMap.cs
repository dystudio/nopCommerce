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
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Price.FixedRate.Data
{
    public class PriceByCountryRecordMap : EntityTypeConfiguration<PriceByCountryRecord>
    {
        public PriceByCountryRecordMap()
        {
            ToTable("ProductPriceByCountry");
            HasKey(m => m.Id);
            Property(m => m.ProductId);
            Property(m => m.CountryId);
            Property(m => m.CurrencyId);
            Property(m => m.Price);
        }
    }
}
