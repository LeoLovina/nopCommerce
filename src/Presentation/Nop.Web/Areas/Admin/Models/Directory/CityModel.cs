using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Directory
{
    /// <summary>
    /// Represents a city model
    /// </summary>
    public partial record CityModel : BaseNopEntityModel, ILocalizedModel<CityLocalizedModel>
    {
        #region Ctor

        public CityModel()
        {
            Locales = new List<CityLocalizedModel>();
        }

        #endregion

        #region Properties

        public int StateProvinceId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Countries.Cities.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Countries.Cities.Fields.Abbreviation")]
        public string Abbreviation { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Countries.Cities.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Countries.Cities.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<CityLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial record CityLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }
        
        [NopResourceDisplayName("Admin.Configuration.Countries.Cities.Fields.Name")]
        public string Name { get; set; }
    }
}