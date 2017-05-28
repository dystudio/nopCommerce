using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.WeChatRestService.Models
{
    public class ConfigureModel : BaseNopModel
    {
        public string ApiToken { get; set; }
        public string Token { get; set; }
        public string EncodingAESKey { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }
}
