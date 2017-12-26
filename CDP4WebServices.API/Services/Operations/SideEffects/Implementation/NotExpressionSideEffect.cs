// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotExpressionSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using CDP4Common.DTO;

    /// <summary>
    /// The purpose of the <see cref="NotExpressionSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public class NotExpressionSideEffect : BooleanExpressionSideEffect<NotExpression>
    {
    }
}