using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations
{
    [NopMigration("2022/01/01 12:00:00:2551770", "Category. Add some new property", UpdateMigrationType.Data, MigrationProcessType.Update)]
    public class AddSomeNewProperty : AutoReversingMigration
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            Create.Column(nameof(Category.SomeNewProperty))
            .OnTable(nameof(Category))
            .AsString(255)
            .Nullable();
        }
    }
}
