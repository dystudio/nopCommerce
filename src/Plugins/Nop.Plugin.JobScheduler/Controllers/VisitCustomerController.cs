using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Plugin.JobScheduler.Models;
using Nop.Plugin.JobScheduler.Services;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;

namespace Nop.Plugin.JobScheduler.Controllers
{
    public class VisitCustomerController : BasePluginController
    {
        private readonly IVisitCustomerService _visitCustomerService;
        private readonly ILocalizationService _localizationService;

        private readonly CustomerSettings _customerSettings;

        public VisitCustomerController(IVisitCustomerService visitCustomerService, ILocalizationService localizationService,
            CustomerSettings customerSettings)
        {
            _visitCustomerService = visitCustomerService;
            _localizationService = localizationService;

            _customerSettings = customerSettings;
        }

        [AdminAuthorize]
        public ActionResult NewCustomer()
        {
            var model = new VisitCustomerListModel
            {
                UsernamesEnabled = _customerSettings.UsernamesEnabled
            };
            return View("~/Plugins/Nop.Plugin.JobScheduler/Views/NewCustomer.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        public ActionResult NewCustomerList(DataSourceRequest command, VisitCustomerListModel model)
        {
            bool shouldFilter = false;
            DateTime createdFromTime = DateTime.MinValue;
            DateTime createdToTime = DateTime.MaxValue;
            if (model.SearchDateTime.HasValue)
            {
                shouldFilter = true;
                createdFromTime = new DateTime(model.SearchDateTime.Value.Year, model.SearchDateTime.Value.Month, model.SearchDateTime.Value.Day, 00, 00, 00);
                createdToTime = new DateTime(model.SearchDateTime.Value.Year, model.SearchDateTime.Value.Month, model.SearchDateTime.Value.Day, 23, 59, 59);
            }
            var visitCustomerModelList = _visitCustomerService.GetAllVisitCustomerList(createdFromTime, createdToTime,
                command.Page - 1, command.PageSize, shouldFilter);

            var visitCustomerCount = _visitCustomerService.GetVisitCustomerCount(createdFromTime, createdToTime, shouldFilter);

            var gridModel = new DataSourceResult
            {
                Data = visitCustomerModelList.Select(x =>
                {
                    var visitCustomerModel = new VisitCustomerModel
                    {
                        Username = x.Username,
                        Email = x.IsRegisterCustomer ? x.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                        RealName = x.RealName,
                        VisitIpAddress = x.VisitIpAddress,
                        IsRegisterCustomer = x.IsRegisterCustomer,
                        VisitedTime = x.VisitedTime.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    return visitCustomerModel;

                }).ToList(),
                Total = visitCustomerModelList.TotalCount,
                ExtraData = new VisitCustomerExtensionModel
                {
                    VisitDateTime = model.SearchDateTime.HasValue ? model.SearchDateTime.Value.ToString("yyyy-MM-dd") : "",
                    VisitCustomerCount = visitCustomerCount.ToString()
                }
            };

            return Json(gridModel);
        }
    }
}
