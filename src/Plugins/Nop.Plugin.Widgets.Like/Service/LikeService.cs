using Nop.Core.Data;
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
    public class LikeService : ILikeService
    {
        private readonly IRepository<LikeInfoTable> _likeInfoRepository;
        public LikeService(IRepository<LikeInfoTable> likeInfoRepository)
        {
            this._likeInfoRepository = likeInfoRepository;
        }


        public void Insert(LikeInfoTable data)
        {
            _likeInfoRepository.Insert(data);
        }


        public void Delete(int id)
        {
            LikeInfoTable data = _likeInfoRepository.Table.Where(x => x.Id == id).FirstOrDefault();
            _likeInfoRepository.Delete(data);
        }




        public void Update(int productId, int customerId,bool delete =false)
        {
            
            if (delete==true)
            {
                var data = _likeInfoRepository.Table.FirstOrDefault(x => x.ProductId == productId && x.CustomerId == customerId);
                _likeInfoRepository.Delete(data);
            }
            else
            {
               var data = new LikeInfoTable()
                {
                    CustomerId = customerId,
                    ProductId = productId,
                    Islike = true
                };
                Insert(data);
            }
        }

        public void DeleteOldData(DateTime time)
        {
            //DateTime Time = time.AddSeconds(-70);
            //var ListData=_currenViewInfoRepository.Table.Where(x => x.LastView<Time).ToList();
            //foreach (var Data in ListData)
            //{
            //    Delete(Data);
            //}
        }

        public int GetCount(int productId)
        {
           return _likeInfoRepository.Table.Count(x => x.ProductId == productId && x.Islike);
        }


        public bool IsAvailable(int productId, int customerId)
        {
            return _likeInfoRepository.Table.Any(x => x.ProductId == productId && x.CustomerId == customerId);
        }

        public LikeInfoTable AvailbilityCheckWithData(int productId, int customerId)
        {
            return _likeInfoRepository.Table.FirstOrDefault(x => x.ProductId == productId && x.CustomerId == customerId);
        }

        public bool IsLikedByCustomer(int productId, int customerId)
        {
            LikeInfoTable singleData= new LikeInfoTable();
            var data = _likeInfoRepository.Table.Where(x => x.ProductId == productId && x.CustomerId == customerId);
            if(data.Count()>1)
            {
                data = data.OrderBy(x => x.Id);
                var dataList = data.ToList();
                for (int i = 0; i < data.Count() - 1; i++)
                {
                    _likeInfoRepository.Delete(dataList[i]);
                }
                singleData = dataList.Last();
            }
            else
            {
                singleData = data.FirstOrDefault();
            }
            return singleData != null && singleData.Islike;
        }

        public int GetLikeCountOfCustomer(int customerId)
        {
            return _likeInfoRepository.Table.Count(x => x.CustomerId == customerId && x.Islike == true);
        }


        public IPagedList<LikeInfoTable> GetLikedProductByCustomer(int customerId,int pageIndex=0,int pageSize=int.MaxValue)
        {
            var query = _likeInfoRepository.Table;
            query = query.Where(x => x.CustomerId == customerId && x.Islike == true);
            query = query.OrderByDescending(x=>x.Id);
            var products = new PagedList<LikeInfoTable>(query, pageIndex, pageSize);
            return products;

        }

        public IPagedList<ProductCount> MostLikedProduct(int pageIndex = 0, int pageSize = int.MaxValue)
        {

            var query = _likeInfoRepository.Table.GroupBy(x => x.ProductId).OrderByDescending(x=>x.Count()).
                      Select(group =>
                          new ProductCount
                          {
                              ProductId = group.Key,
                              Count = group.Count()
                          });
            var products = new PagedList<ProductCount>(query, pageIndex, pageSize);
            return products;
        }
    }
}
