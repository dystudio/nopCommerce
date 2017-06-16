using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Like.Models
{
    public class LikeProductBoxModel
    {
        public int ProductId { get; set; }
        public int LikeCout { get; set; }
        public bool IsLikedByCurrentCustomer { get; set; }
        public bool IsGuestCustomer { get; set; }
    }
}
