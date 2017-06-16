using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Models.Common;

namespace Nop.Plugin.Widgets.Like.Models
{
    public class LikeListModelForCustomer
    {
        public LikeListModelForCustomer()
        {
            LikeList = new List<LikeModelForCustomer>();
            //PagerModel = new PagerModel();
        }
            public IList<LikeModelForCustomer> LikeList { get; set; }
            public PagerModel PagerModel { get; set; }
    }
}
