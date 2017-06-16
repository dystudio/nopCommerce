using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Like.Domain
{
    public class LikeInfoTable : BaseEntity
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public bool IsRegisterdCustomer { get; set; }
        public bool Islike { get; set; }
    }
}
