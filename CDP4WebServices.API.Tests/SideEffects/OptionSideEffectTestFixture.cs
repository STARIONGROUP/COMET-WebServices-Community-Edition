// <copyright file="OptionSideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using CDP4WebServices.API.Services.Operations.SideEffects;
    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="OptionSideEffect"/>
    /// </summary>
    [TestFixture]
    public class OptionSideEffectTestFixture
    {
        private OptionSideEffect optionSideEffect;

        [SetUp]
        public void SetUp()
        {
            this.optionSideEffect = new OptionSideEffect();
        }
    }
}
