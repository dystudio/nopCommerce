using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.Captcha.Models
{
  public  class CaptchaModel
    {
        //model specific fields How much is the sum
        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.Captcha.Captcha")]
        [AllowHtml]
        public string Captcha { get; set; }
    }
}
