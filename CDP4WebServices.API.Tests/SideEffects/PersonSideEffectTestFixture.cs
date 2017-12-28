// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonSideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using CDP4Common;

    using CDP4Orm.Dao;
    using CDP4WebServices.API.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="PersonSideEffect"/> class.
    /// </summary>
    [TestFixture]
    public class PersonSideEffectTestFixture
    {
        private const string ClearTextPassword = "clearText";
        private const string PasswordKey = "Password";
        private const string TestValue = "Test";

        /// <summary>
        /// The <see cref="GlossarySideEffect"/> that is being tested
        /// </summary>
        private PersonSideEffect personSideEffect;
        
        /// <summary>
        /// The mocked <see cref="ITermService"/> used to operate on <see cref="Term"/>s.
        /// </summary>
        private Mock<IPersonDao> personDao;

        private NpgsqlTransaction npgsqlTransaction;

        private ClasslessDTO rawUpdateinfo;

        [SetUp]
        public void SetUp()
        {
            this.npgsqlTransaction = null;

            this.personDao = new Mock<IPersonDao>();

            this.personSideEffect = new PersonSideEffect { PersonDao = this.personDao.Object };
        }

        [Test]
        public void VerifyThatPasswordChangeIsEncapsulated()
        {
            this.rawUpdateinfo = new ClasslessDTO()
                                     {
                                            { TestValue, TestValue },
                                            { PasswordKey, ClearTextPassword }
                                     };

            this.personSideEffect.BeforeUpdate(null, null, null, null, null, this.rawUpdateinfo);
            var expectedPasswordText = string.Format(
                "{1}{0}{1}",
                ClearTextPassword,
                this.personSideEffect.PersonDao.PasswordChangeToken);
            
            // assert same number of entries in raw update info object
            Assert.AreEqual(2, this.rawUpdateinfo.Count);
            Assert.AreEqual(TestValue, this.rawUpdateinfo[TestValue].ToString());

            // assert encapsulated cleartext password
            Assert.AreEqual(expectedPasswordText, this.rawUpdateinfo[PasswordKey].ToString());
        }

        [Test]
        public void VerifyThatNoPasswordChangeIsIgnored()
        {
            this.rawUpdateinfo = new ClasslessDTO()
                                     {
                                             { TestValue, TestValue }
                                     };

            this.personSideEffect.BeforeUpdate(null, null, null, null, null, this.rawUpdateinfo);

            // assert same number of entries in raw update info object
            Assert.AreEqual(1, this.rawUpdateinfo.Count);
            Assert.AreEqual(TestValue, this.rawUpdateinfo[TestValue].ToString());
        }
    }
}
