using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Directory
{
    /// <summary>
    /// Represents a city search model
    /// </summary>
    public partial record CitySearchModel : BaseSearchModel
    {
        #region Properties

        public int StateProvinceId { get; set; }

        #endregion
    }
}