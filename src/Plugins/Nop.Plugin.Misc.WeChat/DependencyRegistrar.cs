using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Fakes;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Plugins;
using Nop.Data;
using Nop.Services.Affiliates;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.ExportImport;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Infrastructure;
using Nop.Services.Installation;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Polls;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using Nop.Services.Tax;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;
using Nop.Plugin.Misc.WeChat.Handler;
using Senparc.Weixin.MessageHandlers;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.Entities.Request;
using System.IO;

namespace Nop.Plugin.Misc.WeChat
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.Register((x, p) =>
            new NopMessageHandler(
                p.Named<Stream>("inputStream"), p.Named<PostModel>("postModel"), p.Named<int>("maxRecordCount")))
                .InstancePerLifetimeScope();

        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 0; }
        }
    }
}
