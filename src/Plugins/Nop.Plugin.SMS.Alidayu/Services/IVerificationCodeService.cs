using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.SMS.Alidayu.Services
{
   public interface IVerificationCodeService
    {
        bool SendVerificationCode(int storeScope, string phoneNumber);
    }
}
