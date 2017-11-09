// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperationSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    /// <summary>
    /// The purpose of the OperationSideEffect interface is to provide a contract to register SideEffect classes.
    /// </summary>
    public interface IOperationSideEffect : IOperationSideEffectFunctions
    {
        /// <summary>
        /// Gets the type name of this generically typed <see cref="OperationSideEffect{T}"/>.
        /// </summary>
        /// <returns>
        /// The type name.
        /// </returns>
        /// <remarks>
        /// The result is used to register this instance in the <see cref="OperationSideEffectProcessor"/> map.
        /// </remarks>
        string RegistryKey { get; }
    }
}