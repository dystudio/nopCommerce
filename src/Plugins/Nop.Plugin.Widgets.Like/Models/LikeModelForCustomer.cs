using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Like.Models
{
    public class LikeModelForCustomer
    {
        public LikeModelForCustomer()
        {
            Picture = new PictureModel();
        }
        public int Id { get; set; }
        public string ProductSeName { get; set; }
        public string ProductName { get; set; }
        public PictureModel Picture { get; set; }
    }
}
