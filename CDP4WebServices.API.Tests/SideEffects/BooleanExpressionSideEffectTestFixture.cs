// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanExpressionSideEffectTestFixture.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;

    using CDP4Common;
    using CDP4Common.DTO;

    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="BooleanExpressionSideEffect"/>
    /// </summary>
    [TestFixture]
    public class BooleanExpressionSideEffectTestFixture
    {
        private NpgsqlTransaction npgsqlTransaction;

        private Mock<ISecurityContext> securityContext;

        private Mock<IParametricConstraintService> parametricConstraintService;

        private NotExpressionSideEffect sideEffect;

        private AndExpression andA;

        private NotExpression notA;

        private NotExpression notB;

        private NotExpression notC;

        private NotExpression notD;

        private NotExpression notE;

        private OrExpression orA;

        private ExclusiveOrExpression exclusiveOrA;

        private RelationalExpression relA;

        private RelationalExpression relB;

        private RelationalExpression relC;

        private RelationalExpression relD;

        private RelationalExpression relE;

        private ParametricConstraint constraintA;

        private ClasslessDTO rawUpdateInfo;

        private const string TestKey = "Term";

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.securityContext = new Mock<ISecurityContext>();

            ////                    AndA
            ////                   /    \
            ////                 NotA    OrA
            ////                 /       /  \
            ////              RelA    NotB  ExclusiveOrA
            ////             /          /      \
            ////           RelB       NotC      NotD
            ////                      /           \
            ////                    RelC          RelD will be updated with itself, NotE outside constraint, AndA, RelE
            this.relA = new RelationalExpression { Iid = Guid.NewGuid() };
            this.relB = new RelationalExpression { Iid = Guid.NewGuid() };
            this.relC = new RelationalExpression { Iid = Guid.NewGuid() };
            this.relD = new RelationalExpression { Iid = Guid.NewGuid() };
            this.relE = new RelationalExpression { Iid = Guid.NewGuid() };

            this.notA = new NotExpression { Iid = Guid.NewGuid(), Term = this.relA.Iid };
            this.notB = new NotExpression { Iid = Guid.NewGuid(), Term = this.relB.Iid };
            this.notC = new NotExpression { Iid = Guid.NewGuid(), Term = this.relC.Iid };
            this.notD = new NotExpression { Iid = Guid.NewGuid(), Term = this.relD.Iid };
            this.notE = new NotExpression { Iid = Guid.NewGuid() }; // ouside constraint

            this.exclusiveOrA =
                new ExclusiveOrExpression
                    {
                        Iid = Guid.NewGuid(),
                        Term = new List<Guid> { this.notC.Iid, this.notD.Iid }
                    };

            this.orA = new OrExpression
                           {
                               Iid = Guid.NewGuid(),
                               Term = new List<Guid> { this.notB.Iid, this.exclusiveOrA.Iid }
                           };

            this.andA = new AndExpression
                            {
                                Iid = Guid.NewGuid(),
                                Term = new List<Guid> { this.notA.Iid, this.orA.Iid }
                            };

            this.constraintA = new ParametricConstraint
                                   {
                                       Iid = Guid.NewGuid(),
                                       Expression =
                                           new List<Guid>
                                               {
                                                   this.relA.Iid,
                                                   this.relB.Iid,
                                                   this.relC.Iid,
                                                   this.relD.Iid,
                                                   this.relE.Iid,
                                                   this.notA.Iid,
                                                   this.notB.Iid,
                                                   this.notC.Iid,
                                                   this.notD.Iid,
                                                   this.exclusiveOrA.Iid,
                                                   this.orA.Iid,
                                                   this.andA.Iid
                                               }
                                   };

            this.parametricConstraintService = new Mock<IParametricConstraintService>();
            this.parametricConstraintService
                .Setup(
                    x => x.GetDeep(
                        this.npgsqlTransaction,
                        It.IsAny<string>(),
                        new List<Guid> { this.constraintA.Iid },
                        It.IsAny<ISecurityContext>())).Returns(
                    new List<Thing>
                        {
                            this.relA,
                            this.relB,
                            this.relC,
                            this.relD,
                            this.relE,
                            this.notA,
                            this.notB,
                            this.notC,
                            this.notD,
                            this.exclusiveOrA,
                            this.orA,
                            this.andA
                        });

            this.sideEffect =
                new NotExpressionSideEffect { ParametricConstraintService = this.parametricConstraintService.Object };
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenTermIsTermItself()
        {
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.notD.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.notD,
                    this.constraintA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenTermIsOutOfChainOrLeadsToCircularDependency()
        {
            // Out of chain
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.notE.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.notD,
                    this.constraintA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));

            // Leads to circular dependency
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.andA.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.notD,
                    this.constraintA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsNotThrownWhenTermWithoutCircularDependency()
        {
            this.rawUpdateInfo = new ClasslessDTO() { { TestKey, this.relE.Iid } };

            Assert.DoesNotThrow(
                () => this.sideEffect.BeforeUpdate(
                    this.notD,
                    this.constraintA,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }
    }
}