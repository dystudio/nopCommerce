using Nop.Web.Framework;

namespace Nop.Plugin.SMS.Alidayu.Models
{
    public class SmsAlidayuModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Sms.Alidayu.Fields.Enabled")]
        public bool Enabled { get; set; }
        public bool Enabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Sms.Alidayu.Fields.AppKey")]
        public string AppKey { get; set; }

        [NopResourceDisplayName("Plugins.Sms.Alidayu.Fields.AppSecret")]
        public string AppSecret { get; set; }

        [NopResourceDisplayName("Plugins.Sms.Alidayu.Fields.PhoneNumber")]
        public string PhoneNumber { get; set; }
        public bool PhoneNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Sms.Alidayu.Fields.TestMessage")]
        public string TestMessage { get; set; }
    }
}