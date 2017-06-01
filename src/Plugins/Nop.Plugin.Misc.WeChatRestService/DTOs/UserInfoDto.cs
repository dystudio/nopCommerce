using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.WeChatRestService.DTOs
{
    public class UserInfoDto
    {
        public string OpenId { get; set; }
        public string NickName { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string AvatarUrl { get; set; }
        public string UnionId { get; set; }
    }
}
