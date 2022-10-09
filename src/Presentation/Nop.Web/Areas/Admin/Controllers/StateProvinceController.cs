using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class StateProvinceController : BaseAdminController
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly IStateProvinceModelFactory _stateProvinceModelFactory;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public StateProvinceController(IAddressService addressService,
            IStateProvinceModelFactory stateProvinceModelFactory,
            IStateProvinceService stateProvinceService,
            ICityService cityService,
            ICustomerActivityService customerActivityService,
            IExportManager exportManager,
            IImportManager importManager,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IStoreMappingService storeMappingService,
            IStoreService storeService)
        {
            _addressService = addressService;
            _stateProvinceModelFactory = stateProvinceModelFactory;
            _stateProvinceService = stateProvinceService;
            _cityService = cityService;
            _customerActivityService = customerActivityService;
            _exportManager = exportManager;
            _importManager = importManager;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(StateProvince stateProvince, StateProvinceModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(stateProvince,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocalesAsync(City city, CityModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(city,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task SaveStoreMappingsAsync(StateProvince stateProvince, StateProvinceModel model)
        {
            //stateProvince.LimitedToStores = model.SelectedStoreIds.Any();
            //await _stateProvinceService.UpdateStateProvinceAsync(stateProvince);

            //var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(stateProvince);
            //var allStores = await _storeService.GetAllStoresAsync();
            //foreach (var store in allStores)
            //{
            //    if (model.SelectedStoreIds.Contains(store.Id))
            //    {
            //        //new store
            //        if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
            //            await _storeMappingService.InsertStoreMappingAsync(stateProvince, store.Id);
            //    }
            //    else
            //    {
            //        //remove store
            //        var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
            //        if (storeMappingToDelete != null)
            //            await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
            //    }
            //}
        }

        #endregion

        #region States/ Provinces

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            //prepare model
            var model = await _stateProvinceModelFactory.PrepareStateProvinceSearchModelAsync(new StateProvinceSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> StateProvinceList(StateProvinceSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _stateProvinceModelFactory.PrepareStateProvinceListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            //prepare model
            var model = await _stateProvinceModelFactory.PrepareStateProvinceModelAsync(new StateProvinceModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(StateProvinceModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var stateProvince = model.ToEntity<StateProvince>();
                await _stateProvinceService.InsertStateProvinceAsync(stateProvince);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewStateProvince",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewStateProvince"), stateProvince.Id), stateProvince);

                //locales
                await UpdateLocalesAsync(stateProvince, model);

                //Stores
                await SaveStoreMappingsAsync(stateProvince, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.StateProvinces.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = stateProvince.Id });
            }

            //prepare model
            model = await _stateProvinceModelFactory.PrepareStateProvinceModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            //try to get a stateProvince with the specified id
            var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(id);
            if (stateProvince == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _stateProvinceModelFactory.PrepareStateProvinceModelAsync(null, stateProvince);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(StateProvinceModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            //try to get a stateProvince with the specified id
            var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(model.Id);
            if (stateProvince == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                stateProvince = model.ToEntity(stateProvince);
                await _stateProvinceService.UpdateStateProvinceAsync(stateProvince);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditStateProvince",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditStateProvince"), stateProvince.Id), stateProvince);

                //locales
                await UpdateLocalesAsync(stateProvince, model);

                //stores
                await SaveStoreMappingsAsync(stateProvince, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.StateProvinces.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = stateProvince.Id });
            }

            //prepare model
            model = await _stateProvinceModelFactory.PrepareStateProvinceModelAsync(model, stateProvince, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            //try to get a stateProvince with the specified id
            var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(id);
            if (stateProvince == null)
                return RedirectToAction("List");

            try
            {
                if (await _addressService.GetAddressTotalByStateProvinceIdAsync(stateProvince.Id) > 0)
                    throw new NopException("The stateProvince can't be deleted. It has associated addresses");

                await _stateProvinceService.DeleteStateProvinceAsync(stateProvince);

                //activity log
                await _customerActivityService.InsertActivityAsync("DeleteStateProvince",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteStateProvince"), stateProvince.Id), stateProvince);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.StateProvinces.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("Edit", new { id = stateProvince.Id });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> PublishSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var stateProvinces = await _stateProvinceService.GetStateProvincesByIdsAsync(selectedIds.ToArray());
            foreach (var stateProvince in stateProvinces)
            {
                stateProvince.Published = true;
                await _stateProvinceService.UpdateStateProvinceAsync(stateProvince);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> UnpublishSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var stateProvinces = await _stateProvinceService.GetStateProvincesByIdsAsync(selectedIds.ToArray());
            foreach (var stateProvince in stateProvinces)
            {
                stateProvince.Published = false;
                await _stateProvinceService.UpdateStateProvinceAsync(stateProvince);
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Cities

        [HttpPost]
        public virtual async Task<IActionResult> Cities(CitySearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return await AccessDeniedDataTablesJson();

            //try to get a stateProvince with the specified id
            var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(searchModel.StateProvinceId)
                ?? throw new ArgumentException("No stateProvince found with the specified id");

            //prepare model
            var model = await _stateProvinceModelFactory.PrepareCityListModelAsync(searchModel, stateProvince);

            return Json(model);
        }

        public virtual async Task<IActionResult> CityCreatePopup(int stateProvinceId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            //try to get a stateProvince with the specified id
            var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(stateProvinceId);
            if (stateProvince == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _stateProvinceModelFactory.PrepareCityModelAsync(new CityModel(), stateProvince, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CityCreatePopup(CityModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            //try to get a stateProvince with the specified id
            var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(model.StateProvinceId);
            if (stateProvince == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var sp = model.ToEntity<City>();

                await _cityService.InsertCityAsync(sp);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewCity",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCity"), sp.Id), sp);

                await UpdateLocalesAsync(sp, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _stateProvinceModelFactory.PrepareCityModelAsync(model, stateProvince, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> CityEditPopup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            //try to get a city with the specified id
            var city = await _cityService.GetCityByIdAsync(id);
            if (city == null)
                return RedirectToAction("List");

            //try to get a stateProvince with the specified id
            var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(city.StateProvinceId);
            if (stateProvince == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _stateProvinceModelFactory.PrepareCityModelAsync(null, stateProvince, city);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> StateEditPopup(CityModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            //try to get a state with the specified id
            var city = await _cityService.GetCityByIdAsync(model.Id);
            if (city == null)
                return RedirectToAction("List");

            //try to get a stateProvince with the specified id
            var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(city.StateProvinceId);
            if (stateProvince == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                city = model.ToEntity(city);
                await _cityService.UpdateCityAsync(city);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditCity",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCity"), city.Id), city);

                await UpdateLocalesAsync(city, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _stateProvinceModelFactory.PrepareCityModelAsync(model, stateProvince, city, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> StateDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            //try to get a state with the specified id
            var state = await _stateProvinceService.GetStateProvinceByIdAsync(id)
                ?? throw new ArgumentException("No state found with the specified id");

            if (await _addressService.GetAddressTotalByStateProvinceIdAsync(state.Id) > 0)
            {
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.Configuration.StateProvinces.Cities.CantDeleteWithAddresses"));
            }

            //int stateProvinceId = state.StateProvinceId;
            await _stateProvinceService.DeleteStateProvinceAsync(state);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteStateProvince",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteStateProvince"), state.Id), state);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> GetStatesByStateProvinceId(string stateProvinceId, bool? addSelectCityItem, bool? addAsterisk)
        {
            //permission validation is not required here

            // This action method gets called via an ajax request
            if (string.IsNullOrEmpty(stateProvinceId))
                throw new ArgumentNullException(nameof(stateProvinceId));

            var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(Convert.ToInt32(stateProvinceId));
            var cities = stateProvince != null ? (await _cityService.GetCitiesByStateProvinceIdAsync(stateProvince.Id, showHidden: true)).ToList() : new List<City>();
            var result = (from c in cities
                          select new { id = c.Id, name = c.Name }).ToList();
            if (addAsterisk.HasValue && addAsterisk.Value)
            {
                //asterisk
                result.Insert(0, new { id = 0, name = "*" });
            }
            else
            {
                if (stateProvince == null)
                {
                    //stateProvince is not selected ("choose stateProvince" item)
                    if (addSelectCityItem.HasValue && addSelectCityItem.Value)
                    {
                        result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Address.SelectState") });
                    }
                    else
                    {
                        result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Address.Other") });
                    }
                }
                else
                {
                    //some stateProvince is selected
                    if (!result.Any())
                    {
                        //stateProvince does not have states
                        result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Address.Other") });
                    }
                    else
                    {
                        //stateProvince has some states
                        if (addSelectCityItem.HasValue && addSelectCityItem.Value)
                        {
                            result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Address.SelectState") });
                        }
                    }
                }
            }

            return Json(result);
        }

        #endregion

        #region Export / import

        public virtual async Task<IActionResult> ExportCsv()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            var fileName = $"cities_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{CommonHelper.GenerateRandomDigitCode(4)}.csv";

            var cities = await _cityService.GetCitiesAsync(true);
            var result = await _exportManager.ExportCitiesToTxtAsync(cities);

            return File(Encoding.UTF8.GetBytes(result), MimeTypes.TextCsv, fileName);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportCsv(IFormFile importcsvfile)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStateProvinces))
                return AccessDeniedView();

            try
            {
                if (importcsvfile != null && importcsvfile.Length > 0)
                {
                    var count = await _importManager.ImportCitiesFromTxtAsync(importcsvfile.OpenReadStream());

                    _notificationService.SuccessNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.StateProvinces.ImportSuccess"), count));

                    return RedirectToAction("List");
                }

                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        #endregion
    }
}