using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.SMS.Alidayu
{
    public class AlidayuRequestUrl
    {
        public static string HttpRequestUrl = @"http://gw.api.taobao.com/router/rest";
        public static string HttpsRequestUrl = @"https://eco.taobao.com/router/rest";
        public static string SandboxHttpRequestUrl = @"http://gw.api.tbsandbox.com/router/rest";
        public static string SandboxHttpsRequestUrl = @"https://gw.api.tbsandbox.com/router/rest";
    }
}
