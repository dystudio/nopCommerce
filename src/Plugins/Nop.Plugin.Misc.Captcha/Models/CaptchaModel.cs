using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Captcha.Models
{
  public  class CaptchaModel
    {
        //model specific fields 
        [Required]
        [Display(Name = "How much is the sum")]
        public string Captcha { get; set; }
    }
}
