using Nop.Core.Configuration;

namespace Nop.Plugin.SMS.Alidayu
{
    public class AlidayuSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the value indicting whether this SMS provider is enabled
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Gets or sets the value ProductName
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// Gets or sets the value indicting whether ssl is enabled
        /// </summary>
        public bool SslEnabled { get; set; }
        /// <summary>
        /// Gets or sets the value indicting whether sandbox is enabled
        /// </summary>
        public bool SandboxEnabled { get; set; }
        /// <summary>
        /// Gets or sets the AppKey
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// Gets or sets the AppSecret
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// Gets or sets the store owner phone number
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Gets or sets the SmsTemplateCode for alidayu
        /// </summary>
        public string SmsTemplateCode { get; set; }
        /// <summary>
        /// Gets or sets the SmsTemplateCodeForVerificationCode for alidayu
        /// </summary>
        public string SmsTemplateCodeForVerificationCode { get; set; }
        

    }
}