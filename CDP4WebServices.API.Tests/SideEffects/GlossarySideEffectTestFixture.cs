// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlossarySideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="GlossarySideEffect"/> class.
    /// </summary>
    [TestFixture]
    public class GlossarySideEffectTestFixture
    {
        /// <summary>
        /// The <see cref="GlossarySideEffect"/> that is being tested
        /// </summary>
        private GlossarySideEffect glossarySideEffect;

        /// <summary>
        /// A mocked <see cref="IPermissionService"/> that returns true
        /// </summary>
        private Mock<IPermissionService> permittingPermissionService;

        /// <summary>
        /// A mocked <see cref="IPermissionService"/> that returns false
        /// </summary>
        private Mock<IPermissionService> denyingPermissionService;

        /// <summary>
        /// The mocked <see cref="ITermService"/> used to operate on <see cref="Term"/>s.
        /// </summary>
        private Mock<ITermService> termService;

        private Mock<ISecurityContext> securityContext;

        private NpgsqlTransaction npgsqlTransaction;

        private Glossary glossary;
        
        [SetUp]
        public void SetUp()
        {
            this.permittingPermissionService = new Mock<IPermissionService>();
            this.permittingPermissionService.Setup(x => x.CanWrite(this.npgsqlTransaction, It.IsAny<Thing>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ISecurityContext>())).Returns(true);

            this.denyingPermissionService = new Mock<IPermissionService>();
            this.denyingPermissionService.Setup(x => x.CanWrite(this.npgsqlTransaction, It.IsAny<Thing>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ISecurityContext>())).Returns(false);

            this.termService = new Mock<ITermService>();
            this.termService.Setup(x => x.UpdateConcept(this.npgsqlTransaction, It.IsAny<string>(), It.IsAny<Term>(), It.IsAny<Thing>())).Returns(true);

            this.securityContext = new Mock<ISecurityContext>();

            this.npgsqlTransaction = null;

            this.glossarySideEffect = new GlossarySideEffect();    
            
            this.glossary = new Glossary(Guid.NewGuid(), 1);
        }

        [Test]
        public void VerifyThatIfTheGlossaryIsNotDeprecateNoServiceCallsAreMade()
        {
            var originalThing = this.glossary.DeepClone<Thing>();
            this.glossary.IsDeprecated = false;
            this.glossary.Term.Add(Guid.NewGuid());
            
            this.glossarySideEffect.PermissionService = this.permittingPermissionService.Object;
            this.glossarySideEffect.TermService = this.termService.Object;

            this.glossarySideEffect.AfterUpdate(this.glossary, null, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.permittingPermissionService.Verify(x => x.CanWrite(this.npgsqlTransaction, originalThing, typeof(Term).Name, "partition", "update", this.securityContext.Object), Times.Never);
            
            this.termService.Verify(x => x.Get(this.npgsqlTransaction, "partition", this.glossary.Term, this.securityContext.Object), Times.Never);

            this.termService.Verify(x => x.UpdateConcept(this.npgsqlTransaction, "partition", It.IsAny<Term>(), It.IsAny<Thing>()), Times.Never);
        }

        [Test]
        public void VerifyThatIfThereAreNoTermsInTheGlossaryNoServiceCalssAreMade()
        {
            var originalThing = this.glossary.DeepClone<Thing>();
            this.glossary.IsDeprecated = true;

            this.glossarySideEffect.PermissionService = this.permittingPermissionService.Object;
            this.glossarySideEffect.TermService = this.termService.Object;

            this.glossarySideEffect.AfterUpdate(this.glossary, null, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.permittingPermissionService.Verify(x => x.CanWrite(this.npgsqlTransaction, originalThing, typeof(Term).Name, "partition", "update", this.securityContext.Object), Times.Never);

            this.termService.Verify(x => x.Get(this.npgsqlTransaction, "partition", this.glossary.Term, this.securityContext.Object), Times.Never);

            this.termService.Verify(x => x.UpdateConcept(this.npgsqlTransaction, "partition", It.IsAny<Term>(), It.IsAny<Thing>()), Times.Never);
        }

        [Test]
        public void VerifyThatIfNoWritePermissionsExistForTermNoTermServiceCallsAreMade()
        {
            var originalThing = this.glossary.DeepClone<Thing>();
            this.glossary.IsDeprecated = true;
            this.glossary.Term.Add(Guid.NewGuid());
            
            this.glossarySideEffect.PermissionService = this.denyingPermissionService.Object;
            this.glossarySideEffect.TermService = this.termService.Object;

            this.glossarySideEffect.AfterUpdate(this.glossary, null, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.denyingPermissionService.Verify(x => x.CanWrite(this.npgsqlTransaction, originalThing, typeof(Term).Name, "partition", "update", this.securityContext.Object), Times.Once);

            this.termService.Verify(x => x.UpdateConcept(this.npgsqlTransaction, "partition", It.IsAny<Term>(), It.IsAny<Thing>()), Times.Never);
        }

        [Test]
        public void VerifyThatTheTermServiceIsInvokedToUpdateTerms()
        {
            var originalThing = this.glossary.DeepClone<Thing>();
            var term1 = new Term(Guid.NewGuid(), 1);
            var term2 = new Term(Guid.NewGuid(), 1);

            var returnedTerms = new List<Term>();
            returnedTerms.Add(term1);
            returnedTerms.Add(term2);
            
            this.glossary.IsDeprecated = true;
            this.glossary.Term.Add(term1.Iid);
            this.glossary.Term.Add(term2.Iid);

            this.glossarySideEffect.PermissionService = this.permittingPermissionService.Object;
            this.glossarySideEffect.TermService = this.termService.Object;

            this.termService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), this.securityContext.Object)).Returns(returnedTerms);

            this.glossarySideEffect.AfterUpdate(this.glossary, null, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);
            
            this.permittingPermissionService.Verify(x => x.CanWrite(this.npgsqlTransaction, originalThing, typeof(Term).Name, "partition", "update", this.securityContext.Object), Times.Once);

            this.termService.Verify(x => x.UpdateConcept(this.npgsqlTransaction, "partition", It.IsAny<Term>(), It.IsAny<Thing>()), Times.Exactly(2));
        }        
    }
}
 