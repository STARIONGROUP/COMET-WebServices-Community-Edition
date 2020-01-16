// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelationalExpressionSideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2015-2020 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;

    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="RelationalExpressionSideEffect"/> class
    /// </summary>
    [TestFixture]
    public class RelationalExpressionSideEffectTestFixture
    {
        private Mock<ISecurityContext> securityContext;
        private NpgsqlTransaction npgsqlTransaction;

        private Mock<IParametricConstraintService> parametricConstraintService;
        private Mock<IRelationalExpressionService> relationalExpressionService;

        private RelationalExpression relationalExpression1;
        private RelationalExpression relationalExpression2;

        private ParametricConstraint parametricConstraint;

        private RelationalExpressionSideEffect sideEffect;

        [SetUp]
        public void Setup()
        {
            this.securityContext = new Mock<ISecurityContext>();
            this.parametricConstraintService = new Mock<IParametricConstraintService>();
            this.relationalExpressionService = new Mock<IRelationalExpressionService>();

            this.npgsqlTransaction = null;

            this.sideEffect = new RelationalExpressionSideEffect
            {
                ParametricConstraintService = this.parametricConstraintService.Object,
                RelationalExpressionService = this.relationalExpressionService.Object
            };

            this.relationalExpression1 = new RelationalExpression(Guid.NewGuid(), 1);
            this.relationalExpression2 = new RelationalExpression(Guid.NewGuid(), 1);

            this.parametricConstraint = new ParametricConstraint(Guid.NewGuid(), 1);
            this.parametricConstraint.Expression.Add(this.relationalExpression1.Iid);
            this.parametricConstraint.Expression.Add(this.relationalExpression2.Iid);

            this.relationalExpressionService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), new[] { this.relationalExpression1.Iid }, this.securityContext.Object)).Returns(new[] { this.relationalExpression1 });
            this.relationalExpressionService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), new[] { this.relationalExpression2.Iid }, this.securityContext.Object)).Returns(new[] { this.relationalExpression2 });
            this.relationalExpressionService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), new[] { this.relationalExpression1.Iid, this.relationalExpression2.Iid }, this.securityContext.Object)).Returns(new[] { this.relationalExpression1, this.relationalExpression2 });

            this.parametricConstraintService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), new[] { this.parametricConstraint.Iid }, this.securityContext.Object)).Returns(new[] { this.parametricConstraint });
        }

        [Test]
        public void VerifyBeforeDeleteSideEffectDoesNotThrowExceptionWhenMultipleRelationalExpressionsArePresent()
        {
            Assert.DoesNotThrow(() => this.sideEffect.BeforeDelete(this.relationalExpression1, this.parametricConstraint, this.npgsqlTransaction, "partition", this.securityContext.Object));
            Assert.DoesNotThrow(() => this.sideEffect.BeforeDelete(this.relationalExpression2, this.parametricConstraint, this.npgsqlTransaction, "partition", this.securityContext.Object));
        }

        [Test]
        public void VerifyBeforeDeleteSideEffectThrowsExceptionLastRelationalExpressionsIsDeleted()
        {
            this.parametricConstraint.Expression.Remove(this.relationalExpression2.Iid);
            Assert.Throws<Cdp4ModelValidationException>(() => this.sideEffect.BeforeDelete(this.relationalExpression1, this.parametricConstraint, this.npgsqlTransaction, "partition", this.securityContext.Object));
            Assert.DoesNotThrow(() => this.sideEffect.BeforeDelete(this.relationalExpression2, this.parametricConstraint, this.npgsqlTransaction, "partition", this.securityContext.Object));
        }
    }
}
