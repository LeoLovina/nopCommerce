using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Localization;

namespace Nop.Services.Directory
{
    /// <summary>
    /// City service
    /// </summary>
    public partial class CityService : ICityService
    {
        #region Fields

        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<City> _cityRepository;

        #endregion

        #region Ctor

        public CityService(IStaticCacheManager staticCacheManager,
            ILocalizationService localizationService,
            IRepository<City> cityRepository)
        {
            _staticCacheManager = staticCacheManager;
            _localizationService = localizationService;
            _cityRepository = cityRepository;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Deletes a city
        /// </summary>
        /// <param name="city">The city</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCityAsync(City city)
        {
            await _cityRepository.DeleteAsync(city);
        }

        /// <summary>
        /// Gets a city
        /// </summary>
        /// <param name="cityId">The city identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the city
        /// </returns>
        public virtual async Task<City> GetCityByIdAsync(int cityId)
        {
            return await _cityRepository.GetByIdAsync(cityId, cache => default);
        }

        /// <summary>
        /// Gets a city by abbreviation
        /// </summary>
        /// <param name="abbreviation">The city abbreviation</param>
        /// <param name="stateProvinceId">StateProvince identifier; pass null to load the state regardless of a country</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the city
        /// </returns>
        public virtual async Task<City> GetCityByAbbreviationAsync(string abbreviation, int? stateProvinceId = null)
        {
            if (string.IsNullOrEmpty(abbreviation))
                return null;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.CitiesByAbbreviationCacheKey
                , abbreviation, stateProvinceId ?? 0);

            var query = _cityRepository.Table.Where(state => state.Abbreviation == abbreviation);

            //filter by country
            if (stateProvinceId.HasValue)
                query = query.Where(state => state.StateProvinceId == stateProvinceId);

            return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
        }

        /// <summary>
        /// Gets a city by address 
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        public virtual async Task<City> GetCityByAddressAsync(Address address)
        {
            return await GetCityByIdAsync(address?.CityId ?? 0);
        }

        /// <summary>
        /// Gets a city collection by country identifier
        /// </summary>
        /// <param name="stateProvinceId">Country identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the states
        /// </returns>
        public virtual async Task<IList<City>> GetCitiesByStateProvinceIdAsync(int stateProvinceId, int languageId = 0, bool showHidden = false)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.CitiesByCountryCacheKey, stateProvinceId, languageId, showHidden);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var query = from sp in _cityRepository.Table
                            orderby sp.DisplayOrder, sp.Name
                            where sp.StateProvinceId == stateProvinceId &&
                            (showHidden || sp.Published)
                            select sp;
                var cities = await query.ToListAsync();

                if (languageId > 0)
                    //we should sort states by localized names when they have the same display order
                    cities = await cities.ToAsyncEnumerable()
                        .OrderBy(c => c.DisplayOrder)
                        .ThenByAwait(async c => await _localizationService.GetLocalizedAsync(c, x => x.Name, languageId))
                        .ToListAsync();

                return cities;
            });
        }

        /// <summary>
        /// Gets all states/provinces
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the states
        /// </returns>
        public virtual async Task<IList<City>> GetCitiesAsync(bool showHidden = false)
        {
            var query = from sp in _cityRepository.Table
                        orderby sp.StateProvinceId, sp.DisplayOrder, sp.Name
                        where showHidden || sp.Published
                        select sp;


            var cities = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.CitiesAllCacheKey, showHidden), async () => await query.ToListAsync());

            return cities;
        }

        /// <summary>
        /// Inserts a city
        /// </summary>
        /// <param name="city">State/province</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCityAsync(City city)
        {
            await _cityRepository.InsertAsync(city);
        }

        /// <summary>
        /// Updates a city
        /// </summary>
        /// <param name="city">State/province</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCityAsync(City city)
        {
            await _cityRepository.UpdateAsync(city);
        }

        #endregion
    }
}