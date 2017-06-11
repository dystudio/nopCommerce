using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.DiscountRules.PurchasedOneProduct
{
    public partial class RouteProvider : IRouteProvider
    {
        #region Methods

        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.DiscountRules.PurchasedOneProduct.Configure",
                 "Plugins/DiscountRulesPurchasedOneProduct/Configure",
                 new { controller = "DiscountRulesPurchasedOneProduct", action = "Configure" },
                 new[] { "Nop.Plugin.DiscountRules.PurchasedOneProduct.Controllers" }
            );
            routes.MapRoute("Plugin.DiscountRules.PurchasedOneProduct.ProductAddPopup",
                 "Plugins/DiscountRulesPurchasedOneProduct/ProductAddPopup",
                 new { controller = "DiscountRulesPurchasedOneProduct", action = "ProductAddPopup" },
                 new[] { "Nop.Plugin.DiscountRules.PurchasedOneProduct.Controllers" }
            );
            routes.MapRoute("Plugin.DiscountRules.PurchasedOneProduct.ProductAddPopupList",
                 "Plugins/DiscountRulesPurchasedOneProduct/ProductAddPopupList",
                 new { controller = "DiscountRulesPurchasedOneProduct", action = "ProductAddPopupList" },
                 new[] { "Nop.Plugin.DiscountRules.PurchasedOneProduct.Controllers" }
            );
            routes.MapRoute("Plugin.DiscountRules.PurchasedOneProduct.LoadProductFriendlyNames",
                 "Plugins/DiscountRulesPurchasedOneProduct/LoadProductFriendlyNames",
                 new { controller = "DiscountRulesPurchasedOneProduct", action = "LoadProductFriendlyNames" },
                 new[] { "Nop.Plugin.DiscountRules.PurchasedOneProduct.Controllers" }
            );
        }

        #endregion

        #region Properties

        public int Priority
        {
            get
            {
                return 0;
            }
        }

        #endregion
    }
}
