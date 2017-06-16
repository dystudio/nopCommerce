using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.Like.Domain;
using Nop.Core;
using Nop.Plugin.Widgets.Like.Models;

namespace Nop.Plugin.Widgets.Like.Service
{
   public interface ILikeService
    {
       void Insert(LikeInfoTable data);
       void Delete(int id);
       void Update(int productId,int customerId, bool delete = false);
       void DeleteOldData(DateTime time);
       int GetCount(int productId);
       bool IsAvailable(int productId, int customerId);
       LikeInfoTable AvailbilityCheckWithData(int productId, int customerId);
       bool IsLikedByCustomer(int productId, int customerId);
       int GetLikeCountOfCustomer(int customerId);
        IPagedList<LikeInfoTable> GetLikedProductByCustomer(int customerId, int pageIndex = 0, int pageSize = int.MaxValue);
        IPagedList<ProductCount> MostLikedProduct(int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
