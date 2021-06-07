// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelationalExpressionSideEffectTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Tests.SideEffects
{
    using System;

    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

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
