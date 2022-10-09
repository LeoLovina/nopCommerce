using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the StateProvince model factory implementation
    /// </summary>
    public partial class StateProvinceModelFactory : IStateProvinceModelFactory
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;

        #endregion

        #region Ctor

        public StateProvinceModelFactory(ICityService cityService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStateProvinceService stateProvinceService)
        {
            _cityService = cityService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _stateProvinceService = stateProvinceService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare state and province search model
        /// </summary>
        /// <param name="searchModel">City search model</param>
        /// <param name="stateProvince">StateProvince</param>
        /// <returns>City search model</returns>
        protected virtual CitySearchModel PrepareCitySearchModel(CitySearchModel searchModel, StateProvince stateProvince)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (stateProvince == null)
                throw new ArgumentNullException(nameof(stateProvince));

            searchModel.StateProvinceId = stateProvince.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare state/province search model
        /// </summary>
        /// <param name="searchModel">state/province search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the state/province search model
        /// </returns>
        public virtual Task<StateProvinceSearchModel> PrepareStateProvinceSearchModelAsync(StateProvinceSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged state/province list model
        /// </summary>
        /// <param name="searchModel">state/province search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the state/province list model
        /// </returns>
        public virtual async Task<StateProvinceListModel> PrepareStateProvinceListModelAsync(StateProvinceSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get StateProvinces
            var stateProvinces = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(countryId:218, showHidden: true)).ToPagedList(searchModel);

            //prepare list model
            var model = await new StateProvinceListModel().PrepareToGridAsync(searchModel, stateProvinces, () =>
            {
                //fill in model values from the entity
                return stateProvinces.SelectAwait(async stateProvince =>
                {
                    var stateProvinceModel = stateProvince.ToModel<StateProvinceModel>();

                    stateProvinceModel.NumberOfCities = (await _cityService.GetCitiesByStateProvinceIdAsync(stateProvince.Id))?.Count ?? 0;

                    return stateProvinceModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare country model
        /// </summary>
        /// <param name="model">StateProvince model</param>
        /// <param name="stateProvince">StateProvince</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the StateProvince model
        /// </returns>
        public virtual async Task<StateProvinceModel> PrepareStateProvinceModelAsync(StateProvinceModel model, StateProvince stateProvince, bool excludeProperties = false)
        {
            Func<StateProvinceLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (stateProvince != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = stateProvince.ToModel<StateProvinceModel>();
                    model.NumberOfCities = (await _cityService.GetCitiesByStateProvinceIdAsync(stateProvince.Id))?.Count ?? 0;
                }

                //prepare nested search model
                PrepareCitySearchModel(model.CitySearchModel, stateProvince);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(stateProvince, entity => entity.Name, languageId, false, false);
                };
            }

            //set default values for the new model
            if (stateProvince == null)
            {
                model.Published = true;
                //model.AllowsBilling = true;
                //model.AllowsShipping = true;
            }

            ////prepare localized models
            //if (!excludeProperties)
            //    model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //////prepare available stores
            //await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, stateProvince, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare paged city list model
        /// </summary>
        /// <param name="searchModel">city search model</param>
        /// <param name="stateProvince">StateProvince</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the city list model
        /// </returns>
        public virtual async Task<CityListModel> PrepareCityListModelAsync(CitySearchModel searchModel, StateProvince stateProvince)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (stateProvince == null)
                throw new ArgumentNullException(nameof(stateProvince));

            //get comments
            var cities = (await _cityService.GetCitiesByStateProvinceIdAsync(stateProvince.Id, showHidden: true)).ToPagedList(searchModel);

            //prepare list model
            var model = new CityListModel().PrepareToGrid(searchModel, cities, () =>
            {
                //fill in model values from the entity
                return cities.Select(city => city.ToModel<CityModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare state and province model
        /// </summary>
        /// <param name="model">City model</param>
        /// <param name="stateProvince">StateProvince</param>
        /// <param name="city">City</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the city model
        /// </returns>
        public virtual async Task<CityModel> PrepareCityModelAsync(CityModel model,
            StateProvince stateProvince, City city, bool excludeProperties = false)
        {
            Func<CityLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (city != null)
            {
                //fill in model values from the entity
                model ??= city.ToModel<CityModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(city, entity => entity.Name, languageId, false, false);
                };
            }

            model.StateProvinceId = stateProvince.Id;

            //set default values for the new model
            if (city == null)
                model.Published = true;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        #endregion
    }
}