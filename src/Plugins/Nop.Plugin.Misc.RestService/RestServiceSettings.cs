using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.RestService
{
    public class RestServiceSettings : ISettings
    {
        public string ApiToken { get; set; }

        public bool SslEnabled { get; set; }
    }
}