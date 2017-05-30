using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.WeChat
{
    public class WeChatSettings : ISettings
    {
        public string Token { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string OriginalId { get; set; }

    }
}