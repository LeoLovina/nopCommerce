using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Directory
{
    /// <summary>
    /// City service interface
    /// </summary>
    public partial interface ICityService
    {
        /// <summary>
        /// Deletes a City
        /// </summary>
        /// <param name="city">The city</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteCityAsync(City city);

        /// <summary>
        /// Gets a city
        /// </summary>
        /// <param name="cityId">The city identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the city
        /// </returns>
        Task<City> GetCityByIdAsync(int cityId);

        /// <summary>
        /// Gets a city by abbreviation
        /// </summary>
        /// <param name="abbreviation">The city abbreviation</param>
        /// <param name="stateProvinceId">Country identifier; pass null to load the state regardless of a country</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the city
        /// </returns>
        Task<City> GetCityByAbbreviationAsync(string abbreviation, int? stateProvinceId = null);

        /// <summary>
        /// Gets a city by address 
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        Task<City> GetCityByAddressAsync(Address address);

        /// <summary>
        /// Gets a city collection by country identifier
        /// </summary>
        /// <param name="stateProvinceId">StateProvince identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the states
        /// </returns>
        Task<IList<City>> GetCitiesByStateProvinceIdAsync(int stateProvinceId, int languageId = 0, bool showHidden = false);

        /// <summary>
        /// Gets all cities
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the states
        /// </returns>
        Task<IList<City>> GetCitiesAsync(bool showHidden = false);

        /// <summary>
        /// Inserts a city
        /// </summary>
        /// <param name="city">City</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertCityAsync(City city);

        /// <summary>
        /// Updates a city
        /// </summary>
        /// <param name="city">City</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateCityAsync(City city);
    }
}
