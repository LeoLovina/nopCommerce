﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Nop.Core;

namespace Nop.Data.Migrations
{
    /// <summary>
    /// Represents the migration manager
    /// </summary>
    public partial class MigrationManager : IMigrationManager
    {
        #region Fields

        private readonly IFilteringMigrationSource _filteringMigrationSource;
        private readonly IMigrationRunner _migrationRunner;
        private readonly IMigrationRunnerConventions _migrationRunnerConventions;
        private readonly Lazy<IVersionLoader> _versionLoader;

        #endregion

        #region Ctor

        public MigrationManager(
            IFilteringMigrationSource filteringMigrationSource,
            IMigrationRunner migrationRunner,
            IMigrationRunnerConventions migrationRunnerConventions,
            Lazy<IVersionLoader> versionLoader)
        {
            _versionLoader = versionLoader;

            _filteringMigrationSource = filteringMigrationSource;
            _migrationRunner = migrationRunner;
            _migrationRunnerConventions = migrationRunnerConventions;
        }

        #endregion

        #region Utils

        /// <summary>
        /// Returns the instances for found types implementing FluentMigrator.IMigration which ready to Up process
        /// </summary>
        /// <param name="assembly">Assembly to find migrations</param>
        /// <param name="migrationProcessType">Type of migration process; pass MigrationProcessType.NoMatter to load all migrations</param>
        /// <returns>The instances for found types implementing FluentMigrator.IMigration</returns>
        protected virtual IEnumerable<IMigrationInfo> GetUpMigrations(Assembly assembly, MigrationProcessType migrationProcessType = MigrationProcessType.NoMatter)
        {
            var migrations = _filteringMigrationSource
                .GetMigrations(t =>
                {
                    var migrationAttribute = t.GetCustomAttribute<NopMigrationAttribute>();
                    
                    if (migrationAttribute is null || _versionLoader.Value.VersionInfo.HasAppliedMigration(migrationAttribute.Version))
                        return false;

                    if (migrationAttribute.TargetMigrationProcess != MigrationProcessType.NoMatter &&
                        migrationProcessType != MigrationProcessType.NoMatter &&
                        migrationProcessType != migrationAttribute.TargetMigrationProcess)
                        return false;

                    return assembly == null || t.Assembly == assembly;

                }) ?? Enumerable.Empty<IMigration>();

            return migrations
                .Select(m => _migrationRunnerConventions.GetMigrationInfoForMigration(m))
                .OrderBy(migration => migration.Version);
        }

        /// <summary>
        /// Returns the instances for found types implementing FluentMigrator.IMigration which ready to Down process
        /// </summary>
        /// <param name="assembly">Assembly to find migrations</param>
        /// <param name="migrationProcessType">Type of migration process; pass MigrationProcessType.NoMatter to load all migrations</param>
        /// <returns>The instances for found types implementing FluentMigrator.IMigration</returns>
        protected virtual IEnumerable<IMigrationInfo> GetDownMigrations(Assembly assembly, MigrationProcessType migrationProcessType = MigrationProcessType.NoMatter)
        {
            var migrations = _filteringMigrationSource
                .GetMigrations(t =>
                {
                    var migrationAttribute = t.GetCustomAttribute<NopMigrationAttribute>();

                    if (migrationAttribute is null || !_versionLoader.Value.VersionInfo.HasAppliedMigration(migrationAttribute.Version))
                        return false;

                    if (migrationAttribute.TargetMigrationProcess != MigrationProcessType.NoMatter &&
                        migrationProcessType != MigrationProcessType.NoMatter &&
                        migrationProcessType != migrationAttribute.TargetMigrationProcess)
                        return false;

                    return assembly == null || t.Assembly == assembly;
                }) ?? Enumerable.Empty<IMigration>();

            return migrations
                .Select(m => _migrationRunnerConventions.GetMigrationInfoForMigration(m))
                .OrderBy(migration => migration.Version);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes an Up for all found unapplied migrations
        /// </summary>
        /// <param name="assembly">Assembly to find migrations</param>
        /// <param name="migrationProcessType">Type of migration process</param>
        public virtual void ApplyUpMigrations(Assembly assembly, MigrationProcessType migrationProcessType = MigrationProcessType.Installation)
        {
            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));

            foreach (var migrationInfo in GetUpMigrations(assembly, migrationProcessType))
            {
                _migrationRunner.Up(migrationInfo.Migration);

#if DEBUG
                if (!string.IsNullOrEmpty(migrationInfo.Description) &&
                    migrationInfo.Description.StartsWith(string.Format(NopMigrationDefaults.UpdateMigrationDescriptionPrefix, NopVersion.FULL_VERSION)))
                    continue;
#endif
                _versionLoader.Value
                    .UpdateVersionInfo(migrationInfo.Version, migrationInfo.Description ?? migrationInfo.Migration.GetType().Name);
            }
        }
        
        /// <summary>
        /// Executes all found (and applied) migrations
        /// </summary>
        /// <param name="assembly">Assembly to find the migration</param>
        public void ApplyDownMigrations(Assembly assembly)
        {
            if(assembly is null)
                throw new ArgumentNullException(nameof(assembly));

            foreach (var migrationInfo in GetDownMigrations(assembly).Reverse())
            {
                _migrationRunner.Down(migrationInfo.Migration);
                _versionLoader.Value.DeleteVersion(migrationInfo.Version);
            }
        }

        #endregion
    }
}