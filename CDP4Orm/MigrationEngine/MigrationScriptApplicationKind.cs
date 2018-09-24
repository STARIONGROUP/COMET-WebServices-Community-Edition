// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationScriptApplicationKind.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.MigrationEngine
{
    /// <summary>
    /// Assertion that determine where a migration script shall be applied on
    /// </summary>
    public enum MigrationScriptApplicationKind
    {
        /// <summary>
        /// Asserts that the migration script shall be applied at "SiteDirectory" level
        /// </summary>
        SiteDirectory,

        /// <summary>
        /// Asserts that the migration script shall be applied at "EngineeringModel" level
        /// </summary>
        EngineeringModel,

        /// <summary>
        /// Asserts that the migration script shall be applied at "Iteration" level
        /// </summary>
        Iteration,

        /// <summary>
        /// Asserts that the migration script shall be applied on all partitions
        /// </summary>
        All
    }
}
