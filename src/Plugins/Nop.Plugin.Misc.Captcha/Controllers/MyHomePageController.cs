using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Nop.Plugin.Misc.Captcha.Models;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;

namespace Nop.Plugin.Misc.Captcha.Controllers
{
    public class MyHomePageController : BasePluginController
    {
        [NopHttpsRequirement(SslRequirement.No)]
        public ActionResult Index()
        {
            return View("~/Plugins/Misc.Captcha/Views/HomeView/Index.cshtml");
        }


    }
}

