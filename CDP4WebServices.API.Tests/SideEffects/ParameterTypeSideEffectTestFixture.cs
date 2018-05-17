// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterTypeSideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using CDP4Common.DTO;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects.Implementation;
    using Moq;
    using Npgsql;
    using NUnit.Framework;

    [TestFixture]
    public class ParameterTypeSideEffectTestFixture
    {
        private ParameterTypeSideEffect parameterTypeSideEffect;
        private Mock<IDefaultValueArrayFactory> defaultValueArrayFactory;

        [SetUp]
        public void SetUp()
        {
            this.defaultValueArrayFactory = new Mock<IDefaultValueArrayFactory>();
            this.parameterTypeSideEffect = new ParameterTypeSideEffect();

            this.parameterTypeSideEffect.DefaultValueArrayFactory = defaultValueArrayFactory.Object;
        }

        [Test]
        public void Verify_that_upon_AfterCreate_DefaultValueArrayFactory_is_reset()
        {
            this.parameterTypeSideEffect.AfterCreate(It.IsAny<ParameterType>(), It.IsAny<Thing>(), It.IsAny<ParameterType>(), It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ISecurityContext>());

            this.defaultValueArrayFactory.Verify(x => x.Reset(), Times.Once);
        }

        [Test]
        public void Verify_that_upon_AfterDelete_DefaultValueArrayFactory_is_reset()
        {
            this.parameterTypeSideEffect.AfterDelete(It.IsAny<ParameterType>(), It.IsAny<Thing>(), It.IsAny<ParameterType>(), It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ISecurityContext>());

            this.defaultValueArrayFactory.Verify(x => x.Reset(), Times.Once);
        }

        [Test]
        public void Verify_that_upon_AfterUpdate_DefaultValueArrayFactory_is_reset()
        {
            this.parameterTypeSideEffect.AfterUpdate(It.IsAny<ParameterType>(), It.IsAny<Thing>(), It.IsAny<ParameterType>(), It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ISecurityContext>());

            this.defaultValueArrayFactory.Verify(x => x.Reset(), Times.Once);
        }
    }
}
