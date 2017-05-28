using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.WeChatRestService.Models
{
    public class APIResponse
    {
        public int StatusCode { get; set; }
        public Object data { get; set; }
    }
}
