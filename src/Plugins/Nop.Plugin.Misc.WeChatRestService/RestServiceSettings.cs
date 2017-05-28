using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.WeChatRestService
{
    public class RestServiceSettings : ISettings
    {
        public string ApiToken { get; set; }
    }
}