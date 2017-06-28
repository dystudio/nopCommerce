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
using Newtonsoft.Json.Linq;
using Nop.Core.Infrastructure;
using Nop.Plugin.Price.FixedRate.Services;
using Nop.Web.Controllers;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Plugin.Price.FixedRate
{
    public class FixedPriceByCountry : ActionFilterAttribute, IFilterProvider
    {
        private readonly IPriceByCountryService _priceByCountryService;

        public FixedPriceByCountry(IPriceByCountryService priceByCountryService)
        {
            this._priceByCountryService = priceByCountryService;
        }
        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            if (((controllerContext.Controller is ProductController && (actionDescriptor.ActionName.Equals("HomepageProducts", StringComparison.InvariantCultureIgnoreCase) || actionDescriptor.ActionName.Equals("ProductDetails", StringComparison.InvariantCultureIgnoreCase))) 
                || (controllerContext.Controller is CatalogController && actionDescriptor.ActionName.Equals("Category", StringComparison.InvariantCultureIgnoreCase))
                || (controllerContext.Controller is ShoppingCartController && (actionDescriptor.ActionName.Equals("Wishlist", StringComparison.InvariantCultureIgnoreCase) || actionDescriptor.ActionName.Equals("ProductDetails_AttributeChange", StringComparison.InvariantCultureIgnoreCase)))
                ))
            //place a breakpoint here so you can see the controller info, remember that this code will catch all
            // actions that fire twice, once before and once after
            // this filter will run the overrides below when the ProductController executes the ProductDetails() action.
            {

                return new List<Filter>() { new Filter(this, FilterScope.Action, 0) };


            }
            return new List<Filter>();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            int i = 10;
            //this is where your custom code goes that will execute before the action executes.
            // put a break point here to see when the productcontroller fires

        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
            int countryId = 0;
            string price = string.Empty;
            var priceByCountryService = EngineContext.Current.Resolve<IPriceByCountryService>();
            if (filterContext.Controller is ProductController)
            {
                if (filterContext.ActionDescriptor.ActionName.Equals("HomepageProducts", StringComparison.InvariantCultureIgnoreCase))
                {
                    countryId = priceByCountryService.GetCountryId();
                    if (countryId > 0)
                    {
                        var model = filterContext.Controller.ViewData.Model as IList<ProductOverviewModel>;
                        foreach (var product in model)
                        {
                            price = priceByCountryService.GetCountrySpecificPrice(countryId, product.Id);
                            product.ProductPrice.Price = string.IsNullOrEmpty(price) ? product.ProductPrice.Price : price;
                        }
                    }
                }
                else if (filterContext.ActionDescriptor.ActionName.Equals("ProductDetails", StringComparison.InvariantCultureIgnoreCase))
                {
                    countryId = priceByCountryService.GetCountryId();
                    if (countryId > 0)
                    {
                        var model = filterContext.Controller.ViewData.Model as ProductDetailsModel;
                        price = priceByCountryService.GetCountrySpecificPrice(countryId, model.Id);
                        model.ProductPrice.Price = string.IsNullOrEmpty(price) ? model.ProductPrice.Price : price; 
                    }
                }
            }
            else if (filterContext.Controller is CatalogController)
            {
                if (filterContext.ActionDescriptor.ActionName.Equals("Category", StringComparison.InvariantCultureIgnoreCase))
                {
                    countryId = priceByCountryService.GetCountryId();
                    if (countryId > 0)
                    {
                        var model = filterContext.Controller.ViewData.Model as CategoryModel;
                        foreach (var product in model.Products)
                        {
                            price = priceByCountryService.GetCountrySpecificPrice(countryId, product.Id);
                            product.ProductPrice.Price = string.IsNullOrEmpty(price) ? product.ProductPrice.Price : price;
                        }
                    }
                }
            }
            else if (filterContext.Controller is ShoppingCartController)
            {
                if (filterContext.ActionDescriptor.ActionName.Equals("Wishlist", StringComparison.InvariantCultureIgnoreCase))
                {
                    countryId = priceByCountryService.GetCountryId();
                    if (countryId > 0)
                    {
                        var model = filterContext.Controller.ViewData.Model as WishlistModel;
                        foreach (var wishListItem in model.Items)
                        {
                            price = priceByCountryService.GetCountrySpecificPrice(countryId, wishListItem.ProductId);
                            if (!string.IsNullOrEmpty(price))
                            {
                                wishListItem.UnitPrice = price;
                                wishListItem.SubTotal = priceByCountryService.GetSubTotal(countryId, wishListItem.Quantity, wishListItem.ProductId);
                            }
                            
                        }
                    }
                }
                else if (filterContext.ActionDescriptor.ActionName.Equals("ProductDetails_AttributeChange", StringComparison.InvariantCultureIgnoreCase))
                {
                    countryId = priceByCountryService.GetCountryId();
                    if (countryId > 0)
                    {
                        if (filterContext.Controller.ViewData.ModelState["productId"] != null)
                        {
                            int productId = Int32.Parse(filterContext.Controller.ViewData.ModelState["productId"].Value.AttemptedValue);
                            price = priceByCountryService.GetCountrySpecificPrice(countryId, productId);
                            var jobject = JObject.FromObject(((JsonResult)filterContext.Result).Data);

                            filterContext.Result = new JsonResult
                            {
                                Data = new { gtin = (string)jobject.Property("gtin").Value, mpn = (string)jobject.Property("mpn").Value, sku = (string)jobject.Property("sku").Value, price = price },
                            };
                        }
                    }
                }
                //else if (filterContext.ActionDescriptor.ActionName.Equals("Cart", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    countryId = priceByCountryService.GetCountryId();
                //    if (countryId > 0)
                //    {
                //        var model = filterContext.Controller.ViewData.Model as ShoppingCartModel;
                //        foreach (var ShoppingCartItem in model.Items)
                //        {
                //            price = priceByCountryService.GetCountrySpecificPrice(countryId, ShoppingCartItem.ProductId);
                //            if (!string.IsNullOrEmpty(price))
                //            {
                //                ShoppingCartItem.UnitPrice = price;
                //                ShoppingCartItem.SubTotal = priceByCountryService.GetSubTotal(countryId, ShoppingCartItem.Quantity, ShoppingCartItem.ProductId);
                //            }

                //        }
                //    }
                //}
                //else if (filterContext.ActionDescriptor.ActionName.Equals("OrderTotals", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    countryId = priceByCountryService.GetCountryId();
                //    if (countryId > 0)
                //    {
                //        var model = filterContext.Controller.ViewData.Model as OrderTotalsModel;
                //        if (model != null)
                //        {
                //            model = priceByCountryService.GetOrderTotal(model, countryId);
                //        }
                //    }
                //}
            }
            base.OnActionExecuted(filterContext);
            //this is where your custom code goes that will execute after the action executes.

        }
    }
}
