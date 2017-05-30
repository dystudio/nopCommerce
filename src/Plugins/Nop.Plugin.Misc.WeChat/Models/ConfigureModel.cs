using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.WeChat.Models
{
    public class ConfigureModel : BaseNopModel
    {
        public string Token { get; set; }
        public string OriginalId { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }
}
