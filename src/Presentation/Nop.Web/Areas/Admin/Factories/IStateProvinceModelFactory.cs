using System.Threading.Tasks;
using Nop.Core.Domain.Directory;
using Nop.Web.Areas.Admin.Models.Directory;
using StateProvince = Nop.Core.Domain.Directory.StateProvince;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the state/province model factory
    /// </summary>
    public partial interface IStateProvinceModelFactory
    {
        /// <summary>
        /// Prepare state/province search model
        /// </summary>
        /// <param name="searchModel">state/province search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the state/province search model
        /// </returns>
        Task<StateProvinceSearchModel> PrepareStateProvinceSearchModelAsync(StateProvinceSearchModel searchModel);

        /// <summary>
        /// Prepare paged state/province list model
        /// </summary>
        /// <param name="searchModel">state/province search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the state/province list model
        /// </returns>
        Task<StateProvinceListModel> PrepareStateProvinceListModelAsync(StateProvinceSearchModel searchModel);

        /// <summary>
        /// Prepare state/province model
        /// </summary>
        /// <param name="model">state/province model</param>
        /// <param name="stateProvince">Country</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country model
        /// </returns>
        Task<StateProvinceModel> PrepareStateProvinceModelAsync(StateProvinceModel model, StateProvince stateProvince, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged state and province list model
        /// </summary>
        /// <param name="searchModel">City search model</param>
        /// <param name="stateProvince">StateProvince</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the state and province list model
        /// </returns>
        Task<CityListModel> PrepareCityListModelAsync(CitySearchModel searchModel, StateProvince stateProvince);

        /// <summary>
        /// Prepare city model
        /// </summary>
        /// <param name="model">city model</param>
        /// <param name="stateProvince">StateProvince</param>
        /// <param name="city">City</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the state and province model
        /// </returns>
        Task<CityModel> PrepareCityModelAsync(CityModel model,
            StateProvince stateProvince, City city, bool excludeProperties = false);
    }
}