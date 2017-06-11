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

namespace Nop.Plugin.Misc.Captcha.Controllers
{
    public class CaptchaPluginController : BasePluginController
    {
        public ActionResult Configure()
        {
            return View("~/Plugins/Misc.Captcha/Views/MyCaptchaPlugin/Configure.cshtml");
        }
        [HttpGet]
        public ActionResult Index()
        {
            return View("~/Plugins/Misc.Captcha/Views/CaptchaView/CaptchaViews.cshtml");
        }
        // GET: Captcha
        [HttpPost]
        public ActionResult Index(CaptchaModel model)
        {
            //validate captcha 
            if (Session["Captcha"] == null || Session["Captcha"].ToString() != model.Captcha)
            {
                ModelState.AddModelError("Captcha", "Wrong value of sum, please try again.");
                //dispay error and generate a new captcha 
               // ViewBag.ErrorMsg = "Wrong value of sum, please try again.";
                return View("~/Plugins/Misc.Captcha/Views/CaptchaView/CaptchaViews.cshtml",model);
            }
            ViewBag.Captcha = "Captcha Verified";
            return View("~/Plugins/Misc.Captcha/Views/CaptchaView/CaptchaViews.cshtml");

        }
        public ActionResult CaptchaImage(string prefix, bool noisy = true)
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            //generate new question 
            int a = rand.Next(10, 99);
            int b = rand.Next(0, 9);
            var captcha = string.Format("{0} + {1} = ?", a, b);

            //store answer 
            Session["Captcha" + prefix] = a + b;

            //image stream 
            FileContentResult img = null;

            using (var mem = new MemoryStream())
            using (var bmp = new Bitmap(130, 30))
            using (var gfx = Graphics.FromImage((Image)bmp))
            {
                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));

                //add noise 
                if (noisy)
                {
                    int i, r, x, y;
                    var pen = new Pen(Color.Yellow);
                    for (i = 1; i < 10; i++)
                    {
                        pen.Color = Color.FromArgb(
                            (rand.Next(0, 255)),
                            (rand.Next(0, 255)),
                            (rand.Next(0, 255)));

                        r = rand.Next(0, (130 / 3));
                        x = rand.Next(0, 130);
                        y = rand.Next(0, 30);
                        int z = x - r;
                        int w = y - r;
                        gfx.DrawEllipse(pen, z, w, r, r);
                    }
                }

                //add question 
                gfx.DrawString(captcha, new Font("Tahoma", 15), Brushes.Gray, 2, 3);

                //render as Jpeg 
                bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
                img = this.File(mem.GetBuffer(), "image/Jpeg");

            }
            return img;
        }

       
    }

}


