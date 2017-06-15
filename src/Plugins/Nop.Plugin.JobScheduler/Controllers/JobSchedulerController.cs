using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Nop.Core.Infrastructure;
using Nop.Plugin.JobScheduler.Common;
using Nop.Plugin.JobScheduler.Domain;
using Nop.Plugin.JobScheduler.Models;
using Nop.Plugin.JobScheduler.Services;
using Nop.Services.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security;
using Quartz;
using Nop.Services.Security;

namespace Nop.Plugin.JobScheduler.Controllers
{
    public class JobSchedulerController : BasePluginController
    {
        private readonly ISchedulerService _schedulerService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        public JobSchedulerController(
            ISchedulerService schedulerService,
            ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            _schedulerService = schedulerService;
            _localizationService = localizationService;
            _permissionService = permissionService;
        }

        /// <summary>
        /// 格式化日期格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        [NonAction]
        private string FormatDateTime(DateTime? dateTime)
        {
            if (dateTime.HasValue)
                return dateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");

            return "";
        }

        [AdminAuthorize]
        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");
            return View("~/Plugins/Nop.Plugin.JobScheduler/Views/List.cshtml");
        }

        [HttpPost]
        [AdminAuthorize]
        public ActionResult SchedulerList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");
            var schedulerList = _schedulerService.GetSchedulers().Where(x => !x.Deleted).ToList();

            var gridModel = new DataSourceResult
            {
                // 作业列表数据不会太多，所以只需要内存分页
                Data = schedulerList.PagedForCommand(command).Select(x =>
                {
                    string timeIntervalDesc = string.Empty;

                    if (x.TimeIntervalId > 0)
                        timeIntervalDesc = _localizationService.GetResource(string.Format("Enums.{0}.{1}", typeof(TimeInterval), x.TimeInterval.ToString()));

                    var model = new SchedulerModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        SystemName = x.SystemName,
                        TimeIntervalId = x.TimeIntervalId.ToString(),
                        TimeIntervalDesc = timeIntervalDesc,
                        IntervalValue = x.IntervalValue,
                        Enabled = x.Enabled,
                        LastRunTime = FormatDateTime(x.LastRunTime),
                        RunJobTime = FormatDateTime(x.RunJobTime)
                    };

                    return model;
                }),
                Total = schedulerList.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult SchedulerUpdate(SchedulerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");
            if (model.Name != null)
                model.Name = model.Name.Trim();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

            var currentScheduler = _schedulerService.GetSchedulerById(model.Id);
            if (currentScheduler == null)
                return Content("未找到相关记录");

            currentScheduler.Name = model.Name;
            currentScheduler.IntervalValue = model.IntervalValue;
            currentScheduler.TimeIntervalId = Convert.ToInt32(model.TimeIntervalId);
            currentScheduler.Enabled = model.Enabled;

            _schedulerService.UpdateScheduler(currentScheduler); // 更新作业设置

            var scheduler = EngineContext.Current.Resolve<IScheduler>();

            var schedulerType = Type.GetType(currentScheduler.SystemName);
            if (schedulerType != null)
            {
                if (currentScheduler.Enabled && !currentScheduler.Deleted)
                {
                    JobKey jobKey = new JobKey(schedulerType.Name);
                    scheduler.DeleteJob(jobKey);

                    // 重新配置作业调度
                    scheduler.InitQuartzScheduler(schedulerType, currentScheduler.TimeInterval, currentScheduler.IntervalValue);

                    // 开始执行作业调度
                    scheduler.Start();

                    SuccessNotification(_localizationService.GetResource("Plugins.JobScheduler.Admin.UpdatedJob.Done"));
                }
                else
                {
                    JobKey jobKey = new JobKey(schedulerType.Name);
                    scheduler.DeleteJob(jobKey);
                }
            }

            Thread.Sleep(500); // 因为修改作业执行时间在作业里面，直接更新会导致作业开始执行时间没法显示在列表（时间差），所以线程挂起0.5秒

            return new NullJsonResult();
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult SchedulerDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");
            var currentScheduler = _schedulerService.GetSchedulerById(id);
            if (currentScheduler == null)
                return Content("未找到相关记录");

            var scheduler = EngineContext.Current.Resolve<IScheduler>();

            var schedulerType = Type.GetType(currentScheduler.SystemName);
            if (schedulerType != null)
            {
                JobKey jobKey = new JobKey(schedulerType.Name);
                scheduler.DeleteJob(jobKey);

                _schedulerService.DeleteScheduler(currentScheduler);

                SuccessNotification(_localizationService.GetResource("Plugins.JobScheduler.Admin.DeletedJob.Done"));
            }

            return new JsonResult();
        }

        public ActionResult RunSchecdulerNow(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");
            var scheduler = EngineContext.Current.Resolve<IScheduler>();

            var currentScheduler = _schedulerService.GetSchedulerById(id);

            if (currentScheduler == null)
                return Content("未找到相关记录");

            if (string.IsNullOrEmpty(currentScheduler.SystemName))
                return RedirectToAction("List");

            currentScheduler.Enabled = true;
            _schedulerService.UpdateScheduler(currentScheduler); // 更新作业设置

            var schedulerType = Type.GetType(currentScheduler.SystemName);
            if (currentScheduler.Enabled && !currentScheduler.Deleted && schedulerType != null)
            {
                JobKey jobKey = new JobKey(schedulerType.Name);
                scheduler.DeleteJob(jobKey);

                // 配置作业调度
                scheduler.InitQuartzScheduler(schedulerType, currentScheduler.TimeInterval, currentScheduler.IntervalValue);

                // 开始执行作业调度
                scheduler.Start();

                SuccessNotification(_localizationService.GetResource("Plugins.JobScheduler.Admin.RunCurrentJob.Done"));
            }

            return RedirectToAction("List");
        }

        [AdminAuthorize]
        public ActionResult ClearAllScheduler()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");
            var scheduler = EngineContext.Current.Resolve<IScheduler>();

            scheduler.Clear(); // 清除Quartz作业列表的所有作业

            var allSchedulerList = _schedulerService.GetSchedulers().Where(x => !x.Deleted).ToList();

            allSchedulerList.ForEach(x =>
            {
                x.Enabled = false;
                x.Deleted = true;
            });
            _schedulerService.DeleteSchedulerBatch(allSchedulerList); // 更新数据库作业列表

            SuccessNotification(_localizationService.GetResource("Plugins.JobScheduler.Admin.ClearJob.Done"));

            return RedirectToAction("List");
        }
    }
}
