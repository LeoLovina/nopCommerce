using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using static LinqToDB.Reflection.Methods.LinqToDB;

namespace Nop.Data.Migrations
{
    [NopMigration("2022/09/16 12:00:00:2551770", "4.50. Add City entity and add CityId to Address", UpdateMigrationType.Data, MigrationProcessType.Update)]
    public class AddCityMigration : Migration
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            var addressTableName = NameCompatibilityManager.GetTableName(typeof(Address));
            if (!Schema.Table(addressTableName).Column(nameof(Address.CityId)).Exists())
            {
                Alter.Table(addressTableName).AddColumn(nameof(Address.CityId)).AsInt32().Nullable();
            }

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(City))).Exists())
            {
                Create.TableFor<City>();
            }
        }

        public override void Down()
        {
            var addressTableName = NameCompatibilityManager.GetTableName(typeof(Address));
            if (Schema.Table(addressTableName).Column(nameof(Address.CityId)).Exists())
            {
                Delete.Column(nameof(Address.CityId)).FromTable(addressTableName);
            }

            var cityTableName = NameCompatibilityManager.GetTableName(typeof(City));
            if (Schema.Table(cityTableName).Exists())
            {
                Delete.Table(cityTableName);
            }
        }
    }
}
