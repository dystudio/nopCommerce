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
    }
}