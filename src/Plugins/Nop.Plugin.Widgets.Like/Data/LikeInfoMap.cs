using Nop.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.Like.Domain;

namespace Nop.Plugin.Widgets.Like.Data
{
    public class LikeInfoMap : NopEntityTypeConfiguration<LikeInfoTable>
    {
        public LikeInfoMap()
        {
            this.ToTable("LikeTable");
            this.HasKey(x => x.Id);
        }
    }
}
