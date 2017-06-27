using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Directory;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
    public partial class CountryController : BaseAdminController
	{
		#region Fields

        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ILocalizationService _localizationService;
	    private readonly IAddressService _addressService;
        private readonly IPermissionService _permissionService;
	    private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public CountryController(ICountryService countryService,
            IStateProvinceService stateProvinceService, 
            ILocalizationService localizationService,
            IAddressService addressService, 
            IPermissionService permissionService,
            ILocalizedEntityService localizedEntityService, 
            ILanguageService languageService,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IExportManager exportManager,
            IImportManager importManager,
            ICustomerActivityService customerActivityService,
            IWorkContext workContext)
        {
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._localizationService = localizationService;
            this._addressService = addressService;
            this._permissionService = permissionService;
            this._localizedEntityService = localizedEntityService;
            this._languageService = languageService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._exportManager = exportManager;
            this._importManager = importManager;
            this._customerActivityService = customerActivityService;
            this._workContext = workContext;
        }

		#endregion 

        #region Utilities
        
        [NonAction]
        protected virtual void UpdateLocales(Country country, CountryModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(country,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdateLocales(StateProvince stateProvince, StateProvinceModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(stateProvince,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void PrepareStoresMappingModel(CountryModel model, Country country, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && country != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(country).ToList();

            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
            }
        }

        [NonAction]
        protected virtual void SaveStoreMappings(Country country, CountryModel model)
        {
            country.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(country);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(country, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region Countries

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual ActionResult CountryList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedKendoGridJson();

            var countries = _countryService.GetAllCountries(showHidden: true);
            var gridModel = new DataSourceResult
            {
                Data = countries.Select(x => x.ToModel()),
                Total = countries.Count
            };

            return Json(gridModel);
        }
        
        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var model = new CountryModel();
            //locales
            AddLocales(_languageService, model.Locales);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //default values
            model.Published = true;
            model.AllowsBilling = true;
            model.AllowsShipping = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(CountryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var country = model.ToEntity();
                _countryService.InsertCountry(country);

                //activity log
                _customerActivityService.InsertActivity("AddNewCountry", _localizationService.GetResource("ActivityLog.AddNewCountry"), country.Id);

                //locales
                UpdateLocales(country, model);
                //Stores
                SaveStoreMappings(country, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Countries.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = country.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //Stores
            PrepareStoresMappingModel(model, null, true);
            return View(model);
        }

        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var country = _countryService.GetCountryById(id);
            if (country == null)
                //No country found with the specified id
                return RedirectToAction("List");

            var model = country.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = country.GetLocalized(x => x.Name, languageId, false, false);
            });
            //Stores
            PrepareStoresMappingModel(model, country, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(CountryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var country = _countryService.GetCountryById(model.Id);
            if (country == null)
                //No country found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                country = model.ToEntity(country);
                _countryService.UpdateCountry(country);

                //activity log
                _customerActivityService.InsertActivity("EditCountry", _localizationService.GetResource("ActivityLog.EditCountry"), country.Id);

                //locales
                UpdateLocales(country, model);
                //Stores
                SaveStoreMappings(country, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Countries.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new {id = country.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //Stores
            PrepareStoresMappingModel(model, country, true);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var country = _countryService.GetCountryById(id);
            if (country == null)
                //No country found with the specified id
                return RedirectToAction("List");

            try
            {
                if (_addressService.GetAddressTotalByCountryId(country.Id) > 0)
                    throw new NopException("The country can't be deleted. It has associated addresses");

                _countryService.DeleteCountry(country);

                //activity log
                _customerActivityService.InsertActivity("DeleteCountry", _localizationService.GetResource("ActivityLog.DeleteCountry"), country.Id);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Countries.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = country.Id });
            }
        }

        [HttpPost]
        public virtual ActionResult PublishSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var countries = _countryService.GetCountriesByIds(selectedIds.ToArray());
                foreach (var country in countries)
                {
                    country.Published = true;
                    _countryService.UpdateCountry(country);
                }
            }

            return Json(new { Result = true });
        }
        [HttpPost]
        public virtual ActionResult UnpublishSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var countries = _countryService.GetCountriesByIds(selectedIds.ToArray());
                foreach (var country in countries)
                {
                    country.Published = false;
                    _countryService.UpdateCountry(country);
                }
            }
            return Json(new { Result = true });
        }

        #endregion

        #region States / provinces
        [HttpGet]
        public ActionResult AllProvinces(string text, int selectedId)
        {
            //StateProvince
            var province = _stateProvinceService.GetStateProvincesByCountryId(selectedId,_workContext.WorkingLanguage.Id, true);
            province.Insert(0, new StateProvince { Name = "[None]", Id = 0 });
            var selectList = new List<SelectListItem>();
            foreach (var c in province)
                selectList.Add(new SelectListItem()
                {
                    Value = c.Id.ToString(),
                    Text = c.GetStateProvinceBreadCrumb(_stateProvinceService),
                    Selected = c.Id == selectedId
                });
            return new JsonResult { Data = selectList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public virtual ActionResult States(int countryId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedKendoGridJson();

            var states = _stateProvinceService.GetStateProvincesByCountryId(countryId, showHidden: true);

            var gridModel = new DataSourceResult
            {
                Data = states.Select(x =>
                {
                    var vmodel = x.ToModel();
                    vmodel.Breadcrumb = x.GetStateProvinceBreadCrumb(_stateProvinceService);
                    return vmodel;
                }),
                Total = states.Count
            };
            return Json(gridModel);
        }

        //create
        public virtual ActionResult StateCreatePopup(int countryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var model = new StateProvinceModel();
            model.CountryId = countryId;
            model.ParentId = 0;
            model.ParentProvinces = new List<SelectListItem> { new SelectListItem { Text = "[None]", Value = "0" } };
            //default value
            model.Published = true;
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult StateCreatePopup(string btnId, string formId, StateProvinceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var country = _countryService.GetCountryById(model.CountryId);
            if (country == null)
                //No country found with the specified id
                return RedirectToAction("List");

            //父级省份提供
            //防止提交后 "dataSource" cannot be null.错误
            //Text = parentCategory.GetCategoryBreadCrumb(_categoryService), Value = parentCategory.Id.ToString()
            //-->
            model.ParentProvinces = new List<SelectListItem> { new SelectListItem { Text = "[None]", Value = "0" } };
            if (model.ParentId >= 0)
            {
                var parentProvinces = _stateProvinceService.GetStateProvinceById(model.ParentId);
                if (parentProvinces != null)
                    model.ParentProvinces.Add(new SelectListItem { Text = parentProvinces.GetStateProvinceBreadCrumb(_stateProvinceService), Value = parentProvinces.Id.ToString() });
                else
                    model.ParentId = 0;
            }
            //<--

            if (ModelState.IsValid)
            {
                var sp = model.ToEntity();

                _stateProvinceService.InsertStateProvince(sp);

                //activity log
                _customerActivityService.InsertActivity("AddNewStateProvince", _localizationService.GetResource("ActivityLog.AddNewStateProvince"), sp.Id);

                UpdateLocales(sp, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual ActionResult StateEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var sp = _stateProvinceService.GetStateProvinceById(id);
            if (sp == null)
                //No state found with the specified id
                return RedirectToAction("List");

            var model = sp.ToModel();

            //-->
            model.ParentProvinces = new List<SelectListItem> { new SelectListItem { Text = "[None]", Value = "0" } };
            if (model.ParentId > 0)
            {
                var parentProvinces = _stateProvinceService.GetStateProvinceById(model.ParentId);
                if (parentProvinces != null)
                    model.ParentProvinces.Add(new SelectListItem { Text = parentProvinces.GetStateProvinceBreadCrumb(_stateProvinceService), Value = parentProvinces.Id.ToString() });
                else
                    model.ParentId = 0;
            }
            //<--

            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = sp.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult StateEditPopup(string btnId, string formId, StateProvinceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var sp = _stateProvinceService.GetStateProvinceById(model.Id);
            if (sp == null)
                //No state found with the specified id
                return RedirectToAction("List");

            //防止提交后 "dataSource" cannot be null.错误
            //-->
            model.ParentProvinces = new List<SelectListItem> { new SelectListItem { Text = "[None]", Value = "0" } };
            //<--

            if (ModelState.IsValid)
            {
                sp = model.ToEntity(sp);
                _stateProvinceService.UpdateStateProvince(sp);

                //activity log
                _customerActivityService.InsertActivity("EditStateProvince", _localizationService.GetResource("ActivityLog.EditStateProvince"), sp.Id);

                UpdateLocales(sp, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult StateDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var state = _stateProvinceService.GetStateProvinceById(id);
            if (state == null)
                throw new ArgumentException("No state found with the specified id");

            if (_addressService.GetAddressTotalByStateProvinceId(state.Id) > 0)
            {
                return Json(new DataSourceResult { Errors = _localizationService.GetResource("Admin.Configuration.Countries.States.CantDeleteWithAddresses") });
            }

            //int countryId = state.CountryId;
            _stateProvinceService.DeleteStateProvince(state);

            //activity log
            _customerActivityService.InsertActivity("DeleteStateProvince", _localizationService.GetResource("ActivityLog.DeleteStateProvince"), state.Id);

            return new NullJsonResult();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public virtual ActionResult GetStatesByCountryId(string countryId,
            bool? addSelectStateItem, bool? addAsterisk)
        {
            //permission validation is not required here


            // This action method gets called via an ajax request
            if (String.IsNullOrEmpty(countryId))
                throw new ArgumentNullException("countryId");

            var country = _countryService.GetCountryById(Convert.ToInt32(countryId));
            var states = country != null ? _stateProvinceService.GetStateProvincesByCountryId(country.Id, showHidden: true).ToList() : new List<StateProvince>();
            var result = (from s in states
                         select new { id = s.Id, name = s.Name }).ToList();
            if (addAsterisk.HasValue && addAsterisk.Value)
            {
                //asterisk
                result.Insert(0, new { id = 0, name = "*" });
            }
            else
            {
                if (country == null)
                {
                    //country is not selected ("choose country" item)
                    if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Address.SelectState") });
                    }
                    else
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Address.OtherNonUS") });
                    }
                }
                else
                {
                    //some country is selected
                    if (!result.Any())
                    {
                        //country does not have states
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Address.OtherNonUS") });
                    }
                    else
                    {
                        //country has some states
                        if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                        {
                            result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Address.SelectState") });
                        }
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Export / import

        public virtual ActionResult ExportCsv()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            string fileName = String.Format("states_{0}_{1}.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));

            var states = _stateProvinceService.GetStateProvinces(true);
            string result = _exportManager.ExportStatesToTxt(states);

            return File(Encoding.UTF8.GetBytes(result), MimeTypes.TextCsv, fileName);
        }

        [HttpPost]
        public virtual ActionResult ImportCsv(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            try
            {
                var file = Request.Files["importcsvfile"];
                if (file != null && file.ContentLength > 0)
                {
                    int count = _importManager.ImportStatesFromTxt(file.InputStream);
                    SuccessNotification(String.Format(_localizationService.GetResource("Admin.Configuration.Countries.ImportSuccess"), count));
                    return RedirectToAction("List");
                }
                ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion
    }
}
