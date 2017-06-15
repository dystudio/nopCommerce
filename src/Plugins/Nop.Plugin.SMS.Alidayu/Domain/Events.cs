using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.SMS.Alidayu.Domain
{
    public class VerificationCodeSentEvent
    {
        public VerificationCodeSentEvent(string phoneNumber, string number)
        {
            this.PhoneNumber = phoneNumber;
            this.Number = number;
        }
        /// <summary>
        /// PhoneNumber
        /// </summary>
        public string PhoneNumber
        {
            get; private set;
        }

        /// <summary>
        /// Number
        /// </summary>
        public string Number
        {
            get; private set;
        }
    }
}
