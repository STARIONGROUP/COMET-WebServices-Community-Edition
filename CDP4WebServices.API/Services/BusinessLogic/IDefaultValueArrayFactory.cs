// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDefaultValueArrayFactory.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;
    using CDP4Common.Types;

    /// <summary>
    /// The purpose of the <see cref="IDefaultValueArrayFactory"/> is to create a default <see cref="ValueArray{String}"/>
    /// where the number of slots is equal to to number of values associated to a <see cref="ParameterType"/> and where
    /// each slot has the value "-"
    /// </summary>
    public interface IDefaultValueArrayFactory : IBusinessLogicService
    {
        /// <summary>
        /// Initializes the <see cref="DefaultValueArrayFactory"/>.
        /// </summary>
        /// <param name="parameterTypes">
        /// The <see cref="ParameterType"/>s that are used to compute the default <see cref="ValueArray{T}"/>
        /// </param>
        /// <param name="parameterTypeComponents">
        /// The <see cref="ParameterTypeComponent"/>s that are used to compute the default <see cref="ValueArray{T}"/>
        /// </param>
        void Initialize(IEnumerable<ParameterType> parameterTypes, IEnumerable<ParameterTypeComponent> parameterTypeComponents);

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
