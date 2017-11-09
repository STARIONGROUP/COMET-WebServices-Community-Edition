// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperationSideEffectProcessor.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    /// <summary>
    /// The purpose of the OperationSideEffectProcessor interface is to provide a specific IoC binding to the <see cref="OperationSideEffectProcessor"/>.
    /// </summary>
    public interface IOperationSideEffectProcessor : IOperationSideEffectFunctions
    {
    }
}