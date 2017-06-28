using Nop.Core.Infrastructure;
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
using Nop.Plugin.Price.FixedRate.Controllers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Framework.Events;
using System;
using System.Text;
using System.Web.Mvc;

namespace Nop.Plugin.Price.FixedRate.Events
{
    class PriceByCountry : IConsumer<AdminTabStripCreated>
    {


        public void HandleEvent(AdminTabStripCreated eventMessage)
        {

            if (eventMessage.TabStripName == "product-edit")
            {
                ProductController fixedPrice = new ProductController();
                int productId = fixedPrice.GetProductId();
                string url = "/FixedPrice/PriceByCountry?productId=" + productId;
                var sb = new StringBuilder();
                sb.Append("<script language=\"javascript\" type=\"text/javascript\">");
                sb.Append(Environment.NewLine);
                sb.Append("$(document).ready(function () {");
                sb.Append(Environment.NewLine);
                sb.Append("var KTabs = $('#product-edit > ul ');");
                sb.Append(Environment.NewLine);
                sb.Append("KTabs.append('<li><a id = pricebycountry aria-expanded= true data-tab-name= tab-PriceByCountry  data-toggle= tab href= #tab-PriceByCountry>Price By Country</a></li>');");
                sb.Append(Environment.NewLine);
                sb.Append("var productId = \"" + productId + "\";");
                sb.Append("var DTabs = $('<div id= tab-PriceByCountry class= tab-pane price></div>');");
                sb.Append("DTabs.appendTo('div.tab-content:first');");
                sb.Append("var id = \"" + productId + "\";");
                sb.Append("$.ajax({");
                sb.Append("type:'POST',");
                sb.Append("url: \"" + url + "\" ,");
                sb.Append("data:\"" + productId + "\",");
                sb.Append("success:function(data){$('#tab-PriceByCountry').append(data);},");
                sb.Append("error: function(){");
                sb.Append("alert('Failure');");
                sb.Append("}");
                sb.Append("});");
                sb.Append("});");
                sb.Append(Environment.NewLine);
                sb.Append("</script>");
                sb.Append(Environment.NewLine);
                eventMessage.BlocksToRender.Add(MvcHtmlString.Create(sb.ToString()));


            }
        }
    }
}
