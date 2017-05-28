using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.WeChatRestService
{
    public class WeChatRestServiceSettings : ISettings
    {
        public string ApiToken { get; set; }
        public string Token { get; set; }
        public string EncodingAESKey { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }

    }
}