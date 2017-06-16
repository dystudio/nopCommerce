using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.Like.Domain;
using Nop.Plugin.Widgets.Like.Models;
using Nop.Plugin.Widgets.Like.Service;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Seo;
using Nop.Web.Framework.Controllers;
using Nop.Core.Domain.Customers;
using Nop.Web.Infrastructure.Cache;
using Nop.Core.Caching;
using Nop.Services.Media;
using Nop.Web.Models.Media;
using Nop.Web.Models.Common;
using System.Collections.Generic;
using Nop.Web.Framework.Kendoui;

namespace Nop.Plugin.Widgets.Like.Controllers
{
    public class WidgetsLikeController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;
        private readonly ICategoryService _categoryService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ILocalizationService _localizationService;
        private readonly ILikeService _likeService;
        private readonly IProductService _productService;
        private readonly ICacheManager _cacheManager;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;
        public WidgetsLikeController(IWorkContext workContext,
            IStoreContext storeContext,
            IStoreService storeService,
            ISettingService settingService,
            IOrderService orderService,
            ILogger logger,
            ICategoryService categoryService,
            IProductAttributeParser productAttributeParser,
            ILocalizationService localizationService,
            ILikeService likeService,
            IProductService productService,
            ICacheManager cacheManager,
            IPictureService pictureService,
            IWebHelper webhelper)


        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._orderService = orderService;
            this._logger = logger;
            this._categoryService = categoryService;
            this._productAttributeParser = productAttributeParser;
            this._localizationService = localizationService;
            this._likeService = likeService;
            this._productService = productService;
            this._cacheManager = cacheManager;
            this._pictureService = pictureService;
            this._webHelper = webhelper;
        }



        #region utility
        public void PrepareLikeModel(IPagedList<LikeInfoTable> data, LikeListModelForCustomer model)
        {
            foreach (var singleData in data)
            {
                var product = _productService.GetProductById(singleData.ProductId);
                if (product == null || product.Deleted)
                    continue;
                var singleModel = new LikeModelForCustomer();
                singleModel.Id = singleData.Id;
                singleModel.ProductName = product.GetLocalized(x => x.Name);
                singleModel.ProductSeName = product.GetSeName();
                int pictureSize = 80;
                //prepare picture model
                var defaultProductPictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY, product.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                singleModel.Picture = _cacheManager.Get(defaultProductPictureCacheKey, () =>
                {
                    var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                    var pictureModel = new PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture)
                    };
                    //"title" attribute
                    pictureModel.Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute)) ?
                        picture.TitleAttribute :
                        string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), singleModel.ProductName);
                    //"alt" attribute
                    pictureModel.AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute)) ?
                        picture.AltAttribute :
                        string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), singleModel.ProductName);

                    return pictureModel;
                });
                model.LikeList.Add(singleModel);
            }
        }

        #endregion






        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            return View("~/Plugins/Widgets.Like/Views/WidgetsLike/Configure.cshtml");
        }

        [AdminAuthorize]
        public ActionResult AdminLike()
        {
            return View("~/Plugins/Widgets.Like/Views/WidgetsLike/AdminLike.cshtml");
        }

        [HttpPost]
        public ActionResult AdminLikeList(DataSourceRequest command)
        {
            var model = _likeService.MostLikedProduct(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = model.Select(x => new 
                {
                    PictureThumbnailUrl = _pictureService.GetPictureUrl(_pictureService.GetPicturesByProductId(x.ProductId, 1).FirstOrDefault(), 80),
                    Name = _productService.GetProductById(x.ProductId).GetSeName(),
                    Count = x.Count
                }),
                Total = model.TotalCount
            };

            return Json(gridModel);
        }



        public ActionResult AddDataToCurrentViewer(int producId)
        {
            if (_likeService.IsAvailable(producId, _workContext.CurrentCustomer.Id))
            {
                _likeService.Update(producId, _workContext.CurrentCustomer.Id);
            }
            else
            {
                var data = new LikeInfoTable
                {
                    ProductId = producId,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    IsRegisterdCustomer = _workContext.CurrentCustomer.IsRegistered()
                };
                _likeService.Insert(data);

            }
            var model = new
            {
                Success = true
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCurrentView(int producId)
        {
            int num = _likeService.GetCount(producId);
            var model = new
            {
                Number = num
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public ActionResult ProductboxInfo(string widgetZone, object additionalData = null)
        {
            int productId = (int)additionalData;
            var model = new LikeProductBoxModel
            {
                ProductId = productId,
                LikeCout = _likeService.GetCount(productId),
                IsLikedByCurrentCustomer = _likeService.IsLikedByCustomer(productId, _workContext.CurrentCustomer.Id),
                IsGuestCustomer = _workContext.CurrentCustomer.IsGuest()
            };

            return PartialView("~/Plugins/Widgets.Like/Views/WidgetsLike/ProductBoxView.cshtml", model);
        }


        [ChildActionOnly]
        public ActionResult LikeHeader(string widgetZone, object additionalData = null)
        {

            var model = new LikeHeaderModel
            {
                IsGuest = _workContext.CurrentCustomer.IsGuest(),
                Count = _likeService.GetLikeCountOfCustomer(_workContext.CurrentCustomer.Id)
            };

            return PartialView("~/Plugins/Widgets.Like/Views/WidgetsLike/LikeHeader.cshtml", model);
        }



        public ActionResult ProductLike(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new Exception("No product found with this id."); ;

            if (!_workContext.CurrentCustomer.IsRegistered())
                throw new Exception("Anonymous Follow is not allowed! Please Login and then try again.");
            string responseError = string.Empty;


            if (_likeService.IsLikedByCustomer(product.Id, _workContext.CurrentCustomer.Id))
                throw new Exception("You are already liked the product. Please the refresh page.");
            try
            {
                _likeService.Update(product.Id, _workContext.CurrentCustomer.Id);
                int productLikeCount = _likeService.GetCount(product.Id);
                int likeCountOfPerson = _likeService.GetLikeCountOfCustomer(_workContext.CurrentCustomer.Id);
                return Json(new { success = true, liked_unliked = 1, productlikecounthtml = productLikeCount.ToString(), message = "Your like  request Success! ", likeproperty = true, likecounthtml = "(" + likeCountOfPerson + ")" });

            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
                responseError += exc.Message;
            }

            return Json(new { success = false, message = responseError });
        }




        public ActionResult ProductUnLike(int productId)
        {

            if (!_workContext.CurrentCustomer.IsRegistered())
                throw new Exception("Anonymous Follow is not allowed! Please Login and then try again.");
            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new Exception("No product found with this id.");

            if (!_likeService.IsLikedByCustomer(product.Id, _workContext.CurrentCustomer.Id))
                throw new Exception("You are already unliked  the product. Please refresh the page.");
            string responseError = string.Empty;

            try
            {

                _likeService.Update(product.Id, _workContext.CurrentCustomer.Id,true);
                int productLikeCount = _likeService.GetCount(product.Id);
                int likeCountOfPerson = _likeService.GetLikeCountOfCustomer(_workContext.CurrentCustomer.Id);
                return Json(new { success = true, liked_unliked = 1, productlikecounthtml = productLikeCount.ToString(), message = "Your unlike  request Success! ", likeproperty = false, likecounthtml = "(" + likeCountOfPerson + ")" });
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
                responseError += exc.Message;
            }

            return Json(new { success = false, message = responseError });
        }


        public ActionResult LikeListForCustomer(int page = 1)
        {
            if (page >= 1)
                page = page - 1;
            if (_workContext.CurrentCustomer.IsGuest())
                return RedirectToRoute("HomePage");
            var list = _likeService.GetLikedProductByCustomer(_workContext.CurrentCustomer.Id, page, 10);
            var model = new LikeListModelForCustomer();
            PrepareLikeModel(list, model);
            model.PagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "Widgets.Like.LikelistForCustomer",
                UseRouteLinks = true,
                RouteValues = new LikeRouteValues { page = page }
            };
            return View("~/Plugins/Widgets.Like/Views/WidgetsLike/LikeListForCustomer.cshtml", model);

        }


        
        [HttpPost, ActionName("LikeListForCustomer")]
        [FormValueRequired("removefromcart")]
        public ActionResult DeleteFromLikelist(FormCollection form, int page = 1)
        {
            if (page >= 1)
                page = page - 1;

            if (_workContext.CurrentCustomer.IsGuest())
                return RedirectToRoute("HomePage");


            var allIdsToRemove = form["removefromcart"] != null
                ? form["removefromcart"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList()
                : new List<int>();

            
            foreach(var id in allIdsToRemove)
            {
                _likeService.Delete(id);
            }

            var list = _likeService.GetLikedProductByCustomer(_workContext.CurrentCustomer.Id, page, 10);
            var model = new LikeListModelForCustomer();
            PrepareLikeModel(list, model);
            model.PagerModel = new PagerModel
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "Widgets.Like.LikelistForCustomer",
                UseRouteLinks = true,
                RouteValues = new LikeRouteValues { page = page }
            };
            if(model.LikeList.Count==0)
                return RedirectToRoute("Widgets.Like.LikelistForCustomer");

            return View("~/Plugins/Widgets.Like/Views/WidgetsLike/LikeListForCustomer.cshtml", model);
        }

    }
}