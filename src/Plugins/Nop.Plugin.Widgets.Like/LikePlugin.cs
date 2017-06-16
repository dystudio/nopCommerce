using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Plugin.Widgets.Like.Data;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Core.Domain.Tasks;
using Nop.Core.Data;
using System.Linq;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Widgets.Like
{
    /// <summary>
    /// Live person provider
    /// </summary>
    public class LikePlugin : BasePlugin, IWidgetPlugin , IAdminMenuPlugin
    {

        private readonly ISettingService _settingService;
        private readonly LikeObjectContext _objectContext;
        private readonly ILocalizationService _localizationService;
        private const string ProductBoxWidget = "productbox_addinfo_before";
        private const string ProductDetailWidget = "productdetails_overview_top";
        private const string HeaderLinkWidget = "header_links_after";
        public LikePlugin(ISettingService settingService, 
            LikeObjectContext objectContext,
            ILocalizationService localizationService)
        {
            this._settingService = settingService;
            this._objectContext = objectContext;
            this._localizationService = localizationService;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string>
            { 
               // "body_end_html_tag_before"
               ProductBoxWidget,
               ProductDetailWidget,
               HeaderLinkWidget
            };
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WidgetsLike";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Widgets.Like.Controllers" }, { "area", null } };
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

            if (widgetZone.Equals(HeaderLinkWidget))
            {
                actionName = "LikeHeader";
                controllerName = "WidgetsLike";
                routeValues = new RouteValueDictionary
                {
                    {"Namespaces", "Nop.Plugin.Widgets.Like.Controllers"},
                    {"area", null},
                    {"widgetZone", widgetZone}
                };
            }
            else
            {
                actionName = "ProductboxInfo";
                controllerName = "WidgetsLike";
                routeValues = new RouteValueDictionary
                {
                    {"Namespaces", "Nop.Plugin.Widgets.Like.Controllers"},
                    {"area", null},
                    {"widgetZone", widgetZone}
                };
            }
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            _objectContext.Install();

            this.AddOrUpdatePluginLocaleResource("Widget.Like.Liked", "Liked", "en-US");
            this.AddOrUpdatePluginLocaleResource("Widget.Like.Count", "Count", "en-US");
            this.AddOrUpdatePluginLocaleResource("Widgets.Like.List", "Like List", "en-US");
            this.AddOrUpdatePluginLocaleResource("Widgets.Like.DeleteSelected", "Delete Selected", "en-US");
            this.AddOrUpdatePluginLocaleResource("Widgets.Like.ListEmpty", "Empty Like List", "en-US");
            this.AddOrUpdatePluginLocaleResource("Widgets.Like.LikeTitle", "Like", "en-US");
            this.AddOrUpdatePluginLocaleResource("Widgets.Like.UnLikeTitle", "Unlike", "en-US");
            this.AddOrUpdatePluginLocaleResource("Widgets.Like.GuestTitle", "Log in to like", "en-US");
            this.AddOrUpdatePluginLocaleResource("Widget.Like.MostLikedProduct", "Most Liked Product", "en-US");

            this.AddOrUpdatePluginLocaleResource("Widget.Like.Liked", "赞过", "zh-CN");
            this.AddOrUpdatePluginLocaleResource("Widget.Like.Count", "个数", "zh-CN");
            this.AddOrUpdatePluginLocaleResource("Widgets.Like.List", "我赞过的", "zh-CN");
            this.AddOrUpdatePluginLocaleResource("Widgets.Like.DeleteSelected", "删除选中", "zh-CN");
            this.AddOrUpdatePluginLocaleResource("Widgets.Like.ListEmpty", "清空列表", "zh-CN");
            this.AddOrUpdatePluginLocaleResource("Widgets.Like.LikeTitle", "赞", "zh-CN");
            this.AddOrUpdatePluginLocaleResource("Widgets.Like.UnLikeTitle", "取消赞", "zh-CN");
            this.AddOrUpdatePluginLocaleResource("Widgets.Like.GuestTitle", "登陆", "zh-CN");
            this.AddOrUpdatePluginLocaleResource("Widget.Like.MostLikedProduct", "赞数最多", "zh-CN");
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _objectContext.Uninstall();
            this.DeletePluginLocaleResource("Widget.Like.Liked");
            this.DeletePluginLocaleResource("Widget.Like.Count");
            this.DeletePluginLocaleResource("Widgets.Like.List");
            this.DeletePluginLocaleResource("Widgets.Like.DeleteSelected");
            this.DeletePluginLocaleResource("Widgets.Like.ListEmpty");
            this.DeletePluginLocaleResource("Widgets.Liike.LikeTitle");
            this.DeletePluginLocaleResource("Widgets.Like.UnLikeTitle");
            this.DeletePluginLocaleResource("Widgets.Like.GuestTitle");
            this.DeletePluginLocaleResource("Widget.Like.MostLikedProduct");

            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var title = _localizationService.GetResource("Widget.Like.MostLikedProduct");
            var menuItemBuilder = new SiteMapNode()
            {
                Visible = true,
                Title = title,
                IconClass = "fa-heart",
                ActionName = "AdminLike",
                ControllerName= "WidgetsLike",
                RouteValues = new RouteValueDictionary() { { "Area", "" } }
            };

            
           

            rootNode.ChildNodes.Add(menuItemBuilder);
        }
    }
}
