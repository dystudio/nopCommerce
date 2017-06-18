using Nop.Core;
using Nop.Core.Plugins;
using Nop.Plugin.Widgets.PriceForSize.Data;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Widgets.PriceForSize
{
    public class PriceForSizePlugin : BasePlugin, IWidgetPlugin, IConsumer<AdminTabStripCreated>
    {
        private readonly PriceForSizeObjectContext _context;
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;

        public PriceForSizePlugin(PriceForSizeObjectContext context, 
            IWebHelper webHelper,
            ILocalizationService localizationService)
        {
            _context = context;
            this._webHelper = webHelper;
            this._localizationService = localizationService;
        }

        public override void Install()
        {
            _context.Install();

            this.AddOrUpdatePluginLocaleResource("Widget.PriceForSize.PriceForSizeTab.Title", "Price for size", "en-US");
            this.AddOrUpdatePluginLocaleResource("Widget.PriceForSize.PriceForSizeTab.Title.hint", "Price for size", "en-US");

            this.AddOrUpdatePluginLocaleResource("Widget.PriceForSize.PriceForSizeTab.Title", "价格设置", "zh-CN");
            this.AddOrUpdatePluginLocaleResource("Widget.PriceForSize.PriceForSizeTab.Title.hint", "价格设置", "zh-CN");

            base.Install();
        }

        public override void Uninstall()
        {
            _context.Uninstall();

            this.DeletePluginLocaleResource("Widget.PriceForSize.PriceForSizeTab.Title");
            this.DeletePluginLocaleResource("Widget.PriceForSize.PriceForSizeTab.Title.hint");

            base.Uninstall();
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { "productdetails_overview_top" };
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = null;
            controllerName = null;
            routeValues = null;
        }

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget zone where it's displayed</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "WidgetsPriceForSize";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.Widgets.PriceForSize.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        public void HandleEvent(AdminTabStripCreated eventMessage)
        {
            if (eventMessage.TabStripName == "product-edit")
            {
                var id = ((Nop.Web.Framework.Mvc.BaseNopEntityModel)eventMessage.Helper.ViewData.Model).Id;

                string url = "/admin/plugins/priceforsize/AdminProduct/" + id.ToString(); //"/ProductKey/GetProductKey?productId=" + productId;

                string tabName = _localizationService.GetResource("Widget.PriceForSize.PriceForSizeTab.Title"); //


                UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                RouteValueDictionary routeValueDictionaries = new RouteValueDictionary()
                                                                    {
                                                                      { "area", null },
                                                                      { "id", id }
                                                                    };
                string str = urlHelper.Action("AdminProduct", "WidgetsPriceForSize", routeValueDictionaries, HttpContext.Current.Request.Url.Scheme);

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<script language=\"javascript\" type=\"text/javascript\">");
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append("  $(document).ready(function() ");
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append("  {");
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append(string.Format("$(\"#product-edit ul:first\").append( \"<li class=''><a aria-expanded='true' href='#tab-myTab' data-toggle='tab' data-tab-name='tab-myTab'>{0}</a></li>\" );", tabName));
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append("$(\"#product-edit .tab-content:first\").append( \"<div id='tab-myTab' class='tab-pane'><div class='panel-group'><div class='panel panel-default'><div class='panel-body' id='price-for-size'></div></div></div></div>\");");
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append("$(\"#price-for-size\").load('" + str + "');");
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append("});");
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append("</script>");
                stringBuilder.Append(Environment.NewLine);

                eventMessage.BlocksToRender.Add(MvcHtmlString.Create(stringBuilder.ToString()));

                //var sb = new StringBuilder();

                //sb.Append("<script language=\"javascript\" type=\"text/javascript\">");
                //sb.Append(Environment.NewLine);
                //sb.Append("$(document).ready(function () {");
                //sb.Append(Environment.NewLine);
                //sb.Append("var kTabs = $('#product-edit').data('kendoTabStrip');");
                ////sb.Append("var kTabs = $('#product-edit').kendoTabStrip().data('kendoTabStrip');");
                //sb.Append(Environment.NewLine);
                //sb.Append(" kTabs.append({ encoded: false, text: \"" + tabName + "\", contentUrl: \"" + url + "\" });");
                //sb.Append(Environment.NewLine);
                //sb.Append("});");
                //sb.Append(Environment.NewLine);
                //sb.Append("</script>");
                //sb.Append(Environment.NewLine);
                //eventMessage.BlocksToRender.Add(MvcHtmlString.Create(sb.ToString()));

            }

        }

    }
}
