// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDefaultValueArrayFactory.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using CDP4Common.Types;
    using CDP4WebServices.API.Services.Authorization;
    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="IDefaultValueArrayFactory"/> is to create a default <see cref="ValueArray{String}"/>
    /// where the number of slots is equal to to number of values associated to a <see cref="ParameterType"/> and where
    /// each slot has the value "-"
    /// </summary>
    public interface IDefaultValueArrayFactory : IBusinessLogicService
    {
        /// <summary>
        /// Gets or sets the <see cref="IParameterTypeService"/>
        /// </summary>
        IParameterTypeService ParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterTypeComponentService"/>
        /// </summary>
        IParameterTypeComponentService ParameterTypeComponentService { get; set; }

        /// <summary>
        /// Initializes the <see cref="DefaultValueArrayFactory"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="securityContext">
        /// The <see cref="ISecurityContext"/> used for permission checking.
        /// </param>
        void Load(NpgsqlTransaction transaction, ISecurityContext securityContext);

        /// <summary>
        /// Resets the <see cref="IDefaultValueArrayFactory"/>.
        /// </summary>
        /// <remarks>
        /// After the <see cref="IDefaultValueArrayFactory"/> has been reset the data needs to be loaded again using the <see cref="Load"/> method.
        /// </remarks>
        void Reset();

        /// <summary>
        /// Creates a <see cref="ValueArray{String}"/> where the number of slots is equal to to number of values associated to a <see cref="ParameterType"/> and where
        /// each slot has the value "-"
        /// </summary>
        /// <param name="parameterTypeIid">
        /// The unique id of the <see cref="ParameterType"/> for which a default <see cref="ValueArray{T}"/> needs to be created.
        /// </param>
        /// <returns>
        /// an instance of <see cref="ValueArray{T}"/>
        /// </returns>
        ValueArray<string> CreateDefaultValueArray(Guid parameterTypeIid);
    }
}