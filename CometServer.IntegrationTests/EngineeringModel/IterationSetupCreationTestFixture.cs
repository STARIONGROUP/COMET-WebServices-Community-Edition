// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationSetupCreationTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2026 Starion Group S.A.
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.IntegrationTests.EngineeringModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Integration tests for the creation of an IterationSetup (and its Iteration) from a source IterationSetup, in
    /// particular from a source that is not the last iteration of the model. The data of such an older iteration is
    /// replayed from the audit tables, which must neither fail nor change, lose or duplicate data.
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    public class IterationSetupCreationTestFixture
    {
        private static readonly Guid SiteDirectoryIid = Guid.Parse("f13de6f8-b03a-46e7-a492-53b2f260f294");
        private static readonly Guid SiteReferenceDataLibraryIid = Guid.Parse("c454c687-ba3e-44c4-86bc-44544b2c7880");
        private static readonly Guid EngineeringModelSetupIid = Guid.Parse("116f6253-89bb-47d4-aa24-d11d197e43c9");
        private static readonly Guid EngineeringModelIid = Guid.Parse("9ec982e4-ef72-4953-aa85-b158a95d8d56");
        private static readonly Guid FirstIterationSetupIid = Guid.Parse("86163b0e-8341-4316-94fc-93ed60ad0dcf");
        private static readonly Guid FirstIterationIid = Guid.Parse("e163c5ad-f32b-4387-b805-f4b34600bc2c");
        private static readonly Guid TestDomainIid = Guid.Parse("0e92edde-fdff-41db-9b1d-f2e484f12535");
        private static readonly Guid AdditionalDomainIid = Guid.Parse("eb759723-14b9-49f4-8611-544d037bb764");
        private static readonly Guid PersonRoleIid = Guid.Parse("2428f4d9-f26d-4112-9d56-1c940748df69");
        private static readonly Guid ParticipantRoleIid = Guid.Parse("ee3ae5ff-ac5e-4957-bab1-7698fba2a267");
        private static readonly Guid TestElementDefinitionIid = Guid.Parse("f73860b2-12f0-43e4-b8b2-c81862c0a159");
        private static readonly Guid TestTwoElementDefinitionIid = Guid.Parse("fe9295c5-af99-494e-86ff-e715837806ae");
        private static readonly Guid TestElementUsageIid = Guid.Parse("75399754-ee45-4bca-b033-63e2019870d1");
        private static readonly Guid TestElementUsageTwoIid = Guid.Parse("f95a1580-e533-4185-b520-208615780afe");

        private CometRestClient admin;

        [SetUp]
        public async Task SetUpAsync()
        {
            await CometServerTestEnvironment.RestoreAsync();
            this.admin = CometServerTestEnvironment.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            this.admin.Dispose();
        }

        [Test]
        public async Task VerifyThatAnIterationCanBeCreatedFromTheLastIteration()
        {
            var firstIteration = await this.GetIterationAsync(EngineeringModelIid, FirstIterationIid);

            var second = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);

            Assert.That(second.IterationNumber, Is.EqualTo(2));

            var secondIteration = await this.GetIterationAsync(EngineeringModelIid, second.IterationIid);

            AssertEquivalent(secondIteration, firstIteration);
            await AssertSingleOpenInsertAuditEntryAsync(EngineeringModelIid);
        }

        [Test]
        public async Task VerifyThatAnIterationCanBeCreatedFromAnOlderIteration()
        {
            var firstIteration = await this.GetIterationAsync(EngineeringModelIid, FirstIterationIid);

            await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);
            var third = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);

            Assert.That(third.IterationNumber, Is.EqualTo(3));

            var thirdIteration = await this.GetIterationAsync(EngineeringModelIid, third.IterationIid);

            AssertEquivalent(thirdIteration, firstIteration);
            await AssertSingleOpenInsertAuditEntryAsync(EngineeringModelIid);
        }

        [Test]
        public async Task VerifyThatAnIterationCanBeCreatedFromAnOlderIterationThatContainsThingsCreatedByAnotherUser()
        {
            var (jane, janePersonIid) = await this.CreateParticipantAsync("jane", "jane");

            using (jane)
            {
                var elementDefinitionIid = Guid.NewGuid();
                var elementUsageIid = Guid.NewGuid();

                await jane.PostAsync(IterationUri(EngineeringModelIid, FirstIterationIid), PostBody(
                    create:
                    [
                        ElementDefinition(elementDefinitionIid, "JaneElementDefinition", AdditionalDomainIid, elementUsageIid),
                        ElementUsage(elementUsageIid, "JaneElementUsage", AdditionalDomainIid, TestElementDefinitionIid)
                    ],
                    update: [IterationElementUpdate(FirstIterationIid, elementDefinitionIid)]));

                var firstIteration = await this.GetIterationAsync(EngineeringModelIid, FirstIterationIid);

                await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);
                var third = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);

                var thirdIteration = await this.GetIterationAsync(EngineeringModelIid, third.IterationIid);

                AssertEquivalent(thirdIteration, firstIteration);

                Assert.Multiple(() =>
                {
                    Assert.That(thirdIteration.Contains(elementDefinitionIid), Is.True);
                    Assert.That(thirdIteration.Contains(elementUsageIid), Is.True);
                });

                await AssertSingleOpenInsertAuditEntryAsync(EngineeringModelIid);

                // jane stays the creator of the replayed things: her insert entry is the only one and stays open
                await Assert.MultipleAsync(async () =>
                {
                    Assert.That(await GetCreatorsAsync(EngineeringModelIid, "ElementDefinition", elementDefinitionIid), Is.EqualTo(new Guid?[] { janePersonIid }));
                    Assert.That(await GetCreatorsAsync(EngineeringModelIid, "ElementUsage", elementUsageIid), Is.EqualTo(new Guid?[] { janePersonIid }));
                    Assert.That(await GetCreatorsAsync(EngineeringModelIid, "ElementDefinition", elementDefinitionIid, true), Is.EqualTo(new Guid?[] { janePersonIid }));
                    Assert.That(await GetCreatorsAsync(EngineeringModelIid, "ElementUsage", elementUsageIid, true), Is.EqualTo(new Guid?[] { janePersonIid }));
                });
            }
        }

        [Test]
        public async Task VerifyThatCreatingAnIterationFromAnOlderIterationDoesNotChangeTheExistingIterations()
        {
            var second = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);
            await this.MakeChangesAsync(second.IterationIid);

            var firstIterationBefore = await this.admin.GetAsync(IterationUri(EngineeringModelIid, FirstIterationIid, true));
            var secondIterationBefore = await this.admin.GetAsync(IterationUri(EngineeringModelIid, second.IterationIid, true));

            await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);

            var firstIterationAfter = await this.admin.GetAsync(IterationUri(EngineeringModelIid, FirstIterationIid, true));
            var secondIterationAfter = await this.admin.GetAsync(IterationUri(EngineeringModelIid, second.IterationIid, true));

            // the existing iterations are compared including their revision numbers
            Assert.Multiple(() =>
            {
                Assert.That(ToSortedJson(firstIterationAfter), Is.EqualTo(ToSortedJson(firstIterationBefore)), "the first iteration changed");
                Assert.That(ToSortedJson(secondIterationAfter), Is.EqualTo(ToSortedJson(secondIterationBefore)), "the second iteration changed");
            });
        }

        [Test]
        public async Task VerifyThatCreatingAnIterationFromAnOlderIterationDoesNotChangeTheExistingIterationsWhenAnotherUserChangedThem()
        {
            var (jane, janePersonIid) = await this.CreateParticipantAsync("jane", "jane");

            using (jane)
            {
                var firstIteration = await this.GetIterationAsync(EngineeringModelIid, FirstIterationIid);
                var second = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);

                // jane renames and deletes things created by the admin, and creates her own things in the second iteration
                var janeElementDefinitionIid = await this.MakeChangesAsync(second.IterationIid, jane);
                var janeElementUsageIid = Guid.NewGuid();

                await jane.PostAsync(IterationUri(EngineeringModelIid, second.IterationIid), PostBody(
                    create: [ElementUsage(janeElementUsageIid, "JaneElementUsage", AdditionalDomainIid, TestElementDefinitionIid)],
                    update:
                    [
                        new JObject
                        {
                            ["classKind"] = "ElementDefinition",
                            ["iid"] = janeElementDefinitionIid.ToString(),
                            ["containedElement"] = new JArray(janeElementUsageIid.ToString())
                        }
                    ]));

                Guid[] elementDefinitionIids = [janeElementDefinitionIid, TestElementDefinitionIid, TestTwoElementDefinitionIid];
                Guid[] elementUsageIids = [janeElementUsageIid, TestElementUsageIid, TestElementUsageTwoIid];

                var firstIterationBefore = await this.admin.GetAsync(IterationUri(EngineeringModelIid, FirstIterationIid, true));
                var secondIterationBefore = await this.admin.GetAsync(IterationUri(EngineeringModelIid, second.IterationIid, true));
                var elementDefinitionAuditBefore = await GetAuditTrailAsync(EngineeringModelIid, "ElementDefinition", elementDefinitionIids);
                var elementUsageAuditBefore = await GetAuditTrailAsync(EngineeringModelIid, "ElementUsage", elementUsageIids);
                var definedThingAuditBefore = await GetAuditTrailAsync(EngineeringModelIid, "DefinedThing", elementDefinitionIids.Concat(elementUsageIids));
                var renamedCreatorsBefore = await GetCreatorsAsync(EngineeringModelIid, "ElementDefinition", TestTwoElementDefinitionIid);

                var third = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);

                var firstIterationAfter = await this.admin.GetAsync(IterationUri(EngineeringModelIid, FirstIterationIid, true));
                var secondIterationAfter = await this.admin.GetAsync(IterationUri(EngineeringModelIid, second.IterationIid, true));
                var elementDefinitionAuditAfter = await GetAuditTrailAsync(EngineeringModelIid, "ElementDefinition", elementDefinitionIids);
                var elementUsageAuditAfter = await GetAuditTrailAsync(EngineeringModelIid, "ElementUsage", elementUsageIids);
                var definedThingAuditAfter = await GetAuditTrailAsync(EngineeringModelIid, "DefinedThing", elementDefinitionIids.Concat(elementUsageIids));
                var thirdIteration = await this.GetIterationAsync(EngineeringModelIid, third.IterationIid);

                Assert.Multiple(() =>
                {
                    // the existing iterations are compared including their revision numbers
                    Assert.That(ToSortedJson(firstIterationAfter), Is.EqualTo(ToSortedJson(firstIterationBefore)), "the first iteration changed");
                    Assert.That(ToSortedJson(secondIterationAfter), Is.EqualTo(ToSortedJson(secondIterationBefore)), "the second iteration changed");

                    // the entries written by jane and the admin before are all kept unchanged
                    Assert.That(elementDefinitionAuditAfter, Is.SupersetOf(elementDefinitionAuditBefore), "ElementDefinition audit entries were changed or lost");
                    Assert.That(elementUsageAuditAfter, Is.SupersetOf(elementUsageAuditBefore), "ElementUsage audit entries were changed or lost");
                    Assert.That(definedThingAuditAfter, Is.SupersetOf(definedThingAuditBefore), "DefinedThing audit entries were changed or lost");

                    // the changes of jane in the second iteration are kept in the second iteration only
                    var secondIteration = new IterationContent(secondIterationAfter);
                    Assert.That(secondIteration.Contains(janeElementDefinitionIid), Is.True);
                    Assert.That(secondIteration.Contains(janeElementUsageIid), Is.True);
                    Assert.That(secondIteration.Contains(TestElementUsageTwoIid), Is.False);
                    Assert.That(secondIteration.Get(TestTwoElementDefinitionIid)["name"].Value<string>(), Is.EqualTo("Renamed in a later iteration"));

                    Assert.That(thirdIteration.Contains(janeElementDefinitionIid), Is.False);
                    Assert.That(thirdIteration.Contains(janeElementUsageIid), Is.False);
                });

                AssertEquivalent(thirdIteration, firstIteration);
                await AssertSingleOpenInsertAuditEntryAsync(EngineeringModelIid);

                // jane stays the creator of her things, that are not replayed, and the replayed thing she renamed keeps
                // its original insert entry, which stays open
                await Assert.MultipleAsync(async () =>
                {
                    Assert.That(await GetCreatorsAsync(EngineeringModelIid, "ElementDefinition", janeElementDefinitionIid), Is.EqualTo(new Guid?[] { janePersonIid }));
                    Assert.That(await GetCreatorsAsync(EngineeringModelIid, "ElementUsage", janeElementUsageIid), Is.EqualTo(new Guid?[] { janePersonIid }));
                    Assert.That(await GetCreatorsAsync(EngineeringModelIid, "ElementDefinition", janeElementDefinitionIid, true), Is.EqualTo(new Guid?[] { janePersonIid }), "the Actor of a thing that is not replayed changed");
                    Assert.That(await GetCreatorsAsync(EngineeringModelIid, "ElementUsage", janeElementUsageIid, true), Is.EqualTo(new Guid?[] { janePersonIid }), "the Actor of a thing that is not replayed changed");
                    Assert.That(await GetCreatorsAsync(EngineeringModelIid, "ElementDefinition", TestTwoElementDefinitionIid), Is.EqualTo(renamedCreatorsBefore));
                    Assert.That(await GetCreatorsAsync(EngineeringModelIid, "ElementDefinition", TestTwoElementDefinitionIid, true), Is.EqualTo(renamedCreatorsBefore), "the Actor of a replayed thing changed");
                    Assert.That(definedThingAuditBefore.Any(x => x.StartsWith($"{TestTwoElementDefinitionIid}|U|") && x.EndsWith($"|{janePersonIid}")), Is.True, "the rename by jane is not in the audit trail");
                    Assert.That(elementUsageAuditBefore.Any(x => x.StartsWith($"{TestElementUsageTwoIid}|D|") && x.EndsWith($"|{janePersonIid}")), Is.True, "the deletion by jane is not in the audit trail");
                });
            }
        }

        [Test]
        public async Task VerifyThatChangesOfALaterIterationAreNotCopiedToAnIterationCreatedFromAnOlderIteration()
        {
            var firstIteration = await this.GetIterationAsync(EngineeringModelIid, FirstIterationIid);

            var second = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);
            var addedElementDefinitionIid = await this.MakeChangesAsync(second.IterationIid);

            var third = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);

            var secondIteration = await this.GetIterationAsync(EngineeringModelIid, second.IterationIid);
            var thirdIteration = await this.GetIterationAsync(EngineeringModelIid, third.IterationIid);

            AssertEquivalent(thirdIteration, firstIteration);

            Assert.Multiple(() =>
            {
                Assert.That(thirdIteration.Contains(addedElementDefinitionIid), Is.False);
                Assert.That(thirdIteration.Get(TestTwoElementDefinitionIid)["name"].Value<string>(), Is.EqualTo("Test 2 Element Definition"));
                Assert.That(thirdIteration.Contains(TestElementUsageTwoIid), Is.True);

                Assert.That(secondIteration.Contains(addedElementDefinitionIid), Is.True);
                Assert.That(secondIteration.Get(TestTwoElementDefinitionIid)["name"].Value<string>(), Is.EqualTo("Renamed in a later iteration"));
                Assert.That(secondIteration.Contains(TestElementUsageTwoIid), Is.False);
            });

            await AssertSingleOpenInsertAuditEntryAsync(EngineeringModelIid);
        }

        [Test]
        public async Task VerifyThatAnIterationCreatedFromAnOlderIterationContainsTheStateOfTheSourceIterationWhenItWasFrozen()
        {
            // create, update and delete things in the first iteration before it is frozen
            var addedElementDefinitionIid = await this.MakeChangesAsync(FirstIterationIid);

            var firstIteration = await this.GetIterationAsync(EngineeringModelIid, FirstIterationIid);

            await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);
            var third = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);

            var thirdIteration = await this.GetIterationAsync(EngineeringModelIid, third.IterationIid);

            AssertEquivalent(thirdIteration, firstIteration);

            Assert.Multiple(() =>
            {
                Assert.That(thirdIteration.Contains(addedElementDefinitionIid), Is.True);
                Assert.That(thirdIteration.Get(TestTwoElementDefinitionIid)["name"].Value<string>(), Is.EqualTo("Renamed in a later iteration"));
                Assert.That(thirdIteration.Contains(TestElementUsageTwoIid), Is.False, "a deleted thing must not be restored");
            });

            await AssertSingleOpenInsertAuditEntryAsync(EngineeringModelIid);
        }

        [Test]
        public async Task VerifyThatIterationsCanBeCreatedRepeatedlyFromOlderIterations()
        {
            var firstIteration = await this.GetIterationAsync(EngineeringModelIid, FirstIterationIid);

            var second = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);
            var addedInSecondIid = await this.MakeChangesAsync(second.IterationIid);
            var secondIteration = await this.GetIterationAsync(EngineeringModelIid, second.IterationIid);

            var third = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);
            var fourth = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);
            var fifth = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, second.IterationSetupIid);
            var sixth = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, third.IterationSetupIid);

            Assert.That(new[] { third.IterationNumber, fourth.IterationNumber, fifth.IterationNumber, sixth.IterationNumber }, Is.EqualTo(new[] { 3, 4, 5, 6 }));

            AssertEquivalent(await this.GetIterationAsync(EngineeringModelIid, FirstIterationIid), firstIteration);
            AssertEquivalent(await this.GetIterationAsync(EngineeringModelIid, second.IterationIid), secondIteration);
            AssertEquivalent(await this.GetIterationAsync(EngineeringModelIid, third.IterationIid), firstIteration);
            AssertEquivalent(await this.GetIterationAsync(EngineeringModelIid, fourth.IterationIid), firstIteration);
            AssertEquivalent(await this.GetIterationAsync(EngineeringModelIid, sixth.IterationIid), firstIteration);

            var fifthIteration = await this.GetIterationAsync(EngineeringModelIid, fifth.IterationIid);
            AssertEquivalent(fifthIteration, secondIteration);
            Assert.That(fifthIteration.Contains(addedInSecondIid), Is.True);

            await AssertSingleOpenInsertAuditEntryAsync(EngineeringModelIid);
        }

        [Test]
        public async Task VerifyThatAnIterationCanBeCreatedFromAnOlderIterationOfAnEmptyModel()
        {
            var (engineeringModelSetupIid, engineeringModelIid) = await this.CreateEmptyEngineeringModelAsync();

            var engineeringModelSetup = (await this.admin.GetAsync($"SiteDirectory/{SiteDirectoryIid}/model/{engineeringModelSetupIid}")).Cast<JObject>().Single();
            var firstIterationSetupIid = Guid.Parse(engineeringModelSetup["iterationSetup"].Single().Value<string>());

            var firstIterationSetup = (await this.admin.GetAsync($"SiteDirectory/{SiteDirectoryIid}/model/{engineeringModelSetupIid}/iterationSetup/{firstIterationSetupIid}")).Cast<JObject>().Single();
            var firstIterationIid = Guid.Parse(firstIterationSetup["iterationIid"].Value<string>());

            var firstIteration = await this.GetIterationAsync(engineeringModelIid, firstIterationIid);

            var second = await this.CreateIterationSetupAsync(engineeringModelSetupIid, firstIterationSetupIid);
            var third = await this.CreateIterationSetupAsync(engineeringModelSetupIid, firstIterationSetupIid);
            var fourth = await this.CreateIterationSetupAsync(engineeringModelSetupIid, second.IterationSetupIid);

            Assert.That(new[] { second.IterationNumber, third.IterationNumber, fourth.IterationNumber }, Is.EqualTo(new[] { 2, 3, 4 }));

            AssertEquivalent(await this.GetIterationAsync(engineeringModelIid, second.IterationIid), firstIteration);
            AssertEquivalent(await this.GetIterationAsync(engineeringModelIid, third.IterationIid), firstIteration);
            AssertEquivalent(await this.GetIterationAsync(engineeringModelIid, fourth.IterationIid), firstIteration);
            await AssertSingleOpenInsertAuditEntryAsync(engineeringModelIid);
        }

        [Test]
        public async Task VerifyThatAnIterationCanBeCreatedFromAnOlderIterationThatWasEmptied()
        {
            var second = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);

            // delete all element definitions (and with them their contained element usages, parameters, ...)
            await this.admin.PostAsync(IterationUri(EngineeringModelIid, second.IterationIid), PostBody(
                delete:
                [
                    new JObject { ["classKind"] = "ElementDefinition", ["iid"] = TestTwoElementDefinitionIid.ToString() },
                    new JObject { ["classKind"] = "ElementDefinition", ["iid"] = TestElementDefinitionIid.ToString() }
                ]));

            var secondIteration = await this.GetIterationAsync(EngineeringModelIid, second.IterationIid);
            Assert.That(secondIteration.Things.Where(x => x["classKind"].Value<string>() is "ElementDefinition" or "ElementUsage"), Is.Empty);

            await this.CreateIterationSetupAsync(EngineeringModelSetupIid, second.IterationSetupIid);
            var fourth = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, second.IterationSetupIid);

            var fourthIteration = await this.GetIterationAsync(EngineeringModelIid, fourth.IterationIid);

            AssertEquivalent(fourthIteration, secondIteration);
            Assert.That(fourthIteration.Things.Where(x => x["classKind"].Value<string>() is "ElementDefinition" or "ElementUsage"), Is.Empty);

            // the first iteration still holds its element definitions
            var firstIteration = await this.GetIterationAsync(EngineeringModelIid, FirstIterationIid);
            Assert.That(firstIteration.Contains(TestElementDefinitionIid) && firstIteration.Contains(TestTwoElementDefinitionIid), Is.True);

            await AssertSingleOpenInsertAuditEntryAsync(EngineeringModelIid);
        }

        [Test]
        public async Task VerifyThatARestoredThingWithoutAnInsertAuditEntryGetsOne()
        {
            // things copied while the audit triggers were disabled (e.g. by the model creator) have no insert audit entry
            await ExecuteSqlAsync($"DELETE FROM \"{IterationSchema(EngineeringModelIid)}\".\"ElementDefinition_Audit\" WHERE \"Action\" = 'I' AND \"Iid\" = @iid;", TestElementDefinitionIid);
            Assert.That(await GetCreatorsAsync(EngineeringModelIid, "ElementDefinition", TestElementDefinitionIid), Is.Empty);

            await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);
            await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);

            var creators = await GetCreatorsAsync(EngineeringModelIid, "ElementDefinition", TestElementDefinitionIid, true);

            Assert.That(creators, Is.EqualTo(new Guid?[] { await GetPersonIidAsync(CometServerTestEnvironment.AdminUserName) }), "the restored thing has no open insert audit entry");
            await AssertSingleOpenInsertAuditEntryAsync(EngineeringModelIid);
        }

        [Test]
        public async Task VerifyThatTheRevisionOfEveryReplayedThingIsSavedOnce()
        {
            var (jane, _) = await this.CreateParticipantAsync("jane", "jane");

            using (jane)
            {
                // a thing created by another person than the one who creates the iteration used to be read once per
                // actor, which made the save of its revision fail with a duplicate key on <Thing>_REV_PK
                var elementDefinitionIid = Guid.NewGuid();

                await jane.PostAsync(IterationUri(EngineeringModelIid, FirstIterationIid), PostBody(
                    create: [ElementDefinition(elementDefinitionIid, "JaneElementDefinition", AdditionalDomainIid)],
                    update: [IterationElementUpdate(FirstIterationIid, elementDefinitionIid)]));

                await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);
                var third = await this.CreateIterationSetupAsync(EngineeringModelSetupIid, FirstIterationSetupIid);

                var thirdIteration = await this.GetIterationAsync(EngineeringModelIid, third.IterationIid);
                var revisionNumber = thirdIteration.Get(elementDefinitionIid)["revisionNumber"].Value<int>();

                var revisions = await CountRevisionsAsync(EngineeringModelIid, "ElementDefinition", elementDefinitionIid, revisionNumber);

                Assert.That(revisions, Is.EqualTo(1));
            }
        }

        /// <summary>
        /// Gets the name of the iteration partition of a model
        /// </summary>
        private static string IterationSchema(Guid engineeringModelIid) => $"Iteration_{engineeringModelIid.ToString().Replace("-", "_")}";

        /// <summary>
        /// Executes SQL directly against the database, with the parameter "iid"
        /// </summary>
        private static async Task ExecuteSqlAsync(string sql, Guid iid)
        {
            await using var connection = new NpgsqlConnection(CometServerTestEnvironment.DatabaseConnectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("iid", iid);
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Asserts that the content of an iteration is equivalent to the expected content and has no duplicate things
        /// </summary>
        private static void AssertEquivalent(IterationContent actual, IterationContent expected)
        {
            var duplicateIids = actual.Iids.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToList();

            Assert.That(duplicateIids, Is.Empty, "the iteration contains duplicate things");

            var differences = actual.DifferencesWith(expected);

            Assert.That(differences, Is.Empty, string.Join(Environment.NewLine, differences));
        }

        /// <summary>
        /// Asserts that no thing of the model has more than one open (ValidTo = infinity) insert ("I") audit entry. The
        /// read queries join the open insert audit entries to resolve the Actor of a thing: a second open insert entry
        /// of another actor makes a thing be read twice.
        /// </summary>
        private static async Task AssertSingleOpenInsertAuditEntryAsync(Guid engineeringModelIid)
        {
            var schemaSuffix = engineeringModelIid.ToString().Replace("-", "_");
            var schemas = new[] { $"EngineeringModel_{schemaSuffix}", $"Iteration_{schemaSuffix}" };
            var duplicates = new List<string>();

            await using var connection = new NpgsqlConnection(CometServerTestEnvironment.DatabaseConnectionString);
            await connection.OpenAsync();

            var auditTables = new List<(string Schema, string Table)>();

            await using (var command = new NpgsqlCommand("SELECT table_schema, table_name FROM information_schema.columns WHERE table_schema = ANY(@schemas) AND table_name LIKE '%\\_Audit' AND column_name = 'Iid';", connection))
            {
                command.Parameters.AddWithValue("schemas", schemas);

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    auditTables.Add((reader.GetString(0), reader.GetString(1)));
                }
            }

            Assert.That(auditTables, Is.Not.Empty, "no audit tables found");

            foreach (var (schema, table) in auditTables)
            {
                await using var command = new NpgsqlCommand($"SELECT \"Iid\"::text FROM \"{schema}\".\"{table}\" WHERE \"Action\" = 'I' AND \"ValidTo\" = 'infinity' GROUP BY \"Iid\" HAVING count(*) > 1;", connection);
                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    duplicates.Add($"{schema}.{table}: {reader.GetString(0)}");
                }
            }

            Assert.That(duplicates, Is.Empty, "things with more than one open insert audit entry");
        }

        /// <summary>
        /// Gets the iid of a person
        /// </summary>
        private static async Task<Guid?> GetPersonIidAsync(string shortName)
        {
            await using var connection = new NpgsqlConnection(CometServerTestEnvironment.DatabaseConnectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand("SELECT \"Iid\" FROM \"SiteDirectory\".\"Person\" WHERE \"ValueTypeDictionary\" -> 'ShortName' = @shortName;", connection);
            command.Parameters.AddWithValue("shortName", shortName);

            return (Guid?)await command.ExecuteScalarAsync();
        }

        /// <summary>
        /// Counts the revision rows of a thing in the iteration partition for one revision number
        /// </summary>
        private static async Task<long> CountRevisionsAsync(Guid engineeringModelIid, string classKind, Guid iid, int revisionNumber)
        {
            await using var connection = new NpgsqlConnection(CometServerTestEnvironment.DatabaseConnectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand($"SELECT count(*) FROM \"{IterationSchema(engineeringModelIid)}\".\"{classKind}_Revision\" WHERE \"Iid\" = @iid AND \"RevisionNumber\" = @revisionNumber;", connection);
            command.Parameters.AddWithValue("iid", iid);
            command.Parameters.AddWithValue("revisionNumber", revisionNumber);

            return (long)await command.ExecuteScalarAsync();
        }

        /// <summary>
        /// Gets the actors of the insert ("I") audit entries of a thing in the iteration partition, oldest first
        /// </summary>
        /// <param name="engineeringModelIid">The iid of the model</param>
        /// <param name="classKind">The class of the thing, the name of its audit table</param>
        /// <param name="iid">The iid of the thing</param>
        /// <param name="openOnly">A value indicating whether only the open (ValidTo = infinity) entries, used to resolve the Actor, are returned</param>
        private static async Task<Guid?[]> GetCreatorsAsync(Guid engineeringModelIid, string classKind, Guid iid, bool openOnly = false)
        {
            var schema = $"Iteration_{engineeringModelIid.ToString().Replace("-", "_")}";
            var creators = new List<Guid?>();

            await using var connection = new NpgsqlConnection(CometServerTestEnvironment.DatabaseConnectionString);
            await connection.OpenAsync();

            var openFilter = openOnly ? " AND \"ValidTo\" = 'infinity'" : string.Empty;

            await using var command = new NpgsqlCommand($"SELECT \"Actor\" FROM \"{schema}\".\"{classKind}_Audit\" WHERE \"Action\" = 'I' AND \"Iid\" = @iid{openFilter} ORDER BY \"ValidFrom\";", connection);
            command.Parameters.AddWithValue("iid", iid);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                creators.Add(reader.IsDBNull(0) ? null : reader.GetGuid(0));
            }

            return creators.ToArray();
        }

        /// <summary>
        /// Gets the audit trail (all audit entries) of things in the iteration partition
        /// </summary>
        /// <returns>
        /// The audit entries, formatted as "Iid|Action|ValidFrom|ValidTo|Actor"
        /// </returns>
        private static async Task<string[]> GetAuditTrailAsync(Guid engineeringModelIid, string classKind, IEnumerable<Guid> iids)
        {
            var schema = $"Iteration_{engineeringModelIid.ToString().Replace("-", "_")}";
            var entries = new List<string>();

            await using var connection = new NpgsqlConnection(CometServerTestEnvironment.DatabaseConnectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(
                $"SELECT \"Iid\", \"Action\", \"ValidFrom\", \"ValidTo\", \"Actor\" FROM \"{schema}\".\"{classKind}_Audit\" WHERE \"Iid\" = ANY(@iids);",
                connection);

            command.Parameters.AddWithValue("iids", iids.ToArray());

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var action = reader.GetString(1);
                var validTo = reader.GetFieldValue<DateTime>(3);
                var actor = reader.IsDBNull(4) ? "null" : reader.GetGuid(4).ToString();
                var formattedValidTo = validTo == DateTime.MaxValue ? "infinity" : validTo.ToString("o");

                entries.Add($"{reader.GetGuid(0)}|{action}|{reader.GetFieldValue<DateTime>(2):o}|{formattedValidTo}|{actor}");
            }

            return entries.Order(StringComparer.Ordinal).ToArray();
        }

        /// <summary>
        /// Creates an ElementDefinition, renames an ElementDefinition and deletes an ElementUsage in an iteration
        /// </summary>
        /// <param name="iterationIid">The iid of the iteration</param>
        /// <param name="client">The client that makes the changes, the admin client when null</param>
        /// <returns>The iid of the created ElementDefinition</returns>
        private async Task<Guid> MakeChangesAsync(Guid iterationIid, CometRestClient client = null)
        {
            var elementDefinitionIid = Guid.NewGuid();

            await (client ?? this.admin).PostAsync(IterationUri(EngineeringModelIid, iterationIid), PostBody(
                delete: [new JObject { ["classKind"] = "ElementUsage", ["iid"] = TestElementUsageTwoIid.ToString() }],
                create: [ElementDefinition(elementDefinitionIid, "AddedElementDefinition", TestDomainIid)],
                update:
                [
                    IterationElementUpdate(iterationIid, elementDefinitionIid),
                    new JObject { ["classKind"] = "ElementDefinition", ["iid"] = TestTwoElementDefinitionIid.ToString(), ["name"] = "Renamed in a later iteration" }
                ]));

            return elementDefinitionIid;
        }

        /// <summary>
        /// Creates a new IterationSetup, and therefore a new Iteration, from a source IterationSetup
        /// </summary>
        private async Task<IterationSetupInfo> CreateIterationSetupAsync(Guid engineeringModelSetupIid, Guid sourceIterationSetupIid)
        {
            var iterationSetupIid = Guid.NewGuid();
            var iterationIid = Guid.NewGuid();

            var result = await this.admin.PostAsync($"SiteDirectory/{SiteDirectoryIid}", PostBody(
                create:
                [
                    new JObject
                    {
                        ["classKind"] = "IterationSetup",
                        ["iid"] = iterationSetupIid.ToString(),
                        ["revisionNumber"] = 0,
                        ["iterationNumber"] = 0,
                        ["iterationIid"] = iterationIid.ToString(),
                        ["description"] = $"created from {sourceIterationSetupIid}",
                        ["sourceIterationSetup"] = sourceIterationSetupIid.ToString(),
                        ["frozenOn"] = null,
                        ["isDeleted"] = false,
                        ["createdOn"] = DateTime.UtcNow.ToString("o")
                    }
                ],
                update:
                [
                    new JObject
                    {
                        ["classKind"] = "EngineeringModelSetup",
                        ["iid"] = engineeringModelSetupIid.ToString(),
                        ["iterationSetup"] = new JArray(iterationSetupIid.ToString())
                    }
                ]));

            var iterationSetup = result.Cast<JObject>().Single(x => x["iid"].Value<string>() == iterationSetupIid.ToString());

            return new IterationSetupInfo(iterationSetupIid, iterationIid, iterationSetup["iterationNumber"].Value<int>());
        }

        /// <summary>
        /// Creates a Person that participates in the test model with all domains
        /// </summary>
        /// <returns>A client authenticated as the new person and the iid of the person</returns>
        private async Task<(CometRestClient Client, Guid PersonIid)> CreateParticipantAsync(string shortName, string password)
        {
            var personIid = Guid.NewGuid();
            var participantIid = Guid.NewGuid();

            await this.admin.PostAsync($"SiteDirectory/{SiteDirectoryIid}", PostBody(
                create:
                [
                    new JObject
                    {
                        ["classKind"] = "Person",
                        ["iid"] = personIid.ToString(),
                        ["revisionNumber"] = 0,
                        ["shortName"] = shortName,
                        ["givenName"] = shortName,
                        ["surname"] = "Doe",
                        ["password"] = password,
                        ["role"] = PersonRoleIid.ToString(),
                        ["defaultDomain"] = AdditionalDomainIid.ToString(),
                        ["isActive"] = true,
                        ["isDeprecated"] = false,
                        ["organization"] = null,
                        ["organizationalUnit"] = null,
                        ["defaultEmailAddress"] = null,
                        ["defaultTelephoneNumber"] = null,
                        ["emailAddress"] = new JArray(),
                        ["telephoneNumber"] = new JArray(),
                        ["userPreference"] = new JArray()
                    },
                    new JObject
                    {
                        ["classKind"] = "Participant",
                        ["iid"] = participantIid.ToString(),
                        ["revisionNumber"] = 0,
                        ["person"] = personIid.ToString(),
                        ["role"] = ParticipantRoleIid.ToString(),
                        ["domain"] = new JArray(TestDomainIid.ToString(), AdditionalDomainIid.ToString()),
                        ["selectedDomain"] = AdditionalDomainIid.ToString(),
                        ["isActive"] = true
                    }
                ],
                update:
                [
                    new JObject { ["classKind"] = "SiteDirectory", ["iid"] = SiteDirectoryIid.ToString(), ["person"] = new JArray(personIid.ToString()) },
                    new JObject { ["classKind"] = "EngineeringModelSetup", ["iid"] = EngineeringModelSetupIid.ToString(), ["participant"] = new JArray(participantIid.ToString()) }
                ]));

            return (CometServerTestEnvironment.CreateClient(shortName, password), personIid);
        }

        /// <summary>
        /// Creates a new EngineeringModelSetup without source, which creates a model with an empty first iteration
        /// </summary>
        private async Task<(Guid EngineeringModelSetupIid, Guid EngineeringModelIid)> CreateEmptyEngineeringModelAsync()
        {
            var engineeringModelSetupIid = Guid.NewGuid();
            var engineeringModelIid = Guid.NewGuid();
            var modelReferenceDataLibraryIid = Guid.NewGuid();

            await this.admin.PostAsync($"SiteDirectory/{SiteDirectoryIid}", PostBody(
                create:
                [
                    new JObject
                    {
                        ["classKind"] = "EngineeringModelSetup",
                        ["iid"] = engineeringModelSetupIid.ToString(),
                        ["revisionNumber"] = 0,
                        ["engineeringModelIid"] = engineeringModelIid.ToString(),
                        ["name"] = "Empty Model",
                        ["shortName"] = "EmptyModel",
                        ["kind"] = "STUDY_MODEL",
                        ["studyPhase"] = "PREPARATION_PHASE",
                        ["sourceEngineeringModelSetupIid"] = null,
                        ["requiredRdl"] = new JArray(modelReferenceDataLibraryIid.ToString()),
                        ["activeDomain"] = new JArray(),
                        ["participant"] = new JArray(),
                        ["iterationSetup"] = new JArray(),
                        ["alias"] = new JArray(),
                        ["definition"] = new JArray(),
                        ["hyperLink"] = new JArray()
                    },
                    new JObject
                    {
                        ["classKind"] = "ModelReferenceDataLibrary",
                        ["iid"] = modelReferenceDataLibraryIid.ToString(),
                        ["revisionNumber"] = 0,
                        ["name"] = "Empty Model RDL",
                        ["shortName"] = "EmptyModelRDL",
                        ["requiredRdl"] = SiteReferenceDataLibraryIid.ToString(),
                        ["alias"] = new JArray(),
                        ["baseQuantityKind"] = new JArray(),
                        ["baseUnit"] = new JArray(),
                        ["constant"] = new JArray(),
                        ["definedCategory"] = new JArray(),
                        ["definition"] = new JArray(),
                        ["fileType"] = new JArray(),
                        ["glossary"] = new JArray(),
                        ["hyperLink"] = new JArray(),
                        ["parameterType"] = new JArray(),
                        ["referenceSource"] = new JArray(),
                        ["rule"] = new JArray(),
                        ["scale"] = new JArray(),
                        ["unit"] = new JArray(),
                        ["unitPrefix"] = new JArray()
                    }
                ],
                update: [new JObject { ["classKind"] = "SiteDirectory", ["iid"] = SiteDirectoryIid.ToString(), ["model"] = new JArray(engineeringModelSetupIid.ToString()) }]));

            return (engineeringModelSetupIid, engineeringModelIid);
        }

        private async Task<IterationContent> GetIterationAsync(Guid engineeringModelIid, Guid iterationIid)
        {
            return new IterationContent(await this.admin.GetAsync(IterationUri(engineeringModelIid, iterationIid, true)));
        }

        private static string IterationUri(Guid engineeringModelIid, Guid iterationIid, bool deep = false)
        {
            return $"EngineeringModel/{engineeringModelIid}/iteration/{iterationIid}{(deep ? "?extent=deep" : string.Empty)}";
        }

        private static string ToSortedJson(JArray things)
        {
            return string.Join(Environment.NewLine, things.Select(x => x.ToString(Newtonsoft.Json.Formatting.None)).OrderBy(x => x, StringComparer.Ordinal));
        }

        private static JObject PostBody(IEnumerable<JObject> delete = null, IEnumerable<JObject> create = null, IEnumerable<JObject> update = null)
        {
            return new JObject
            {
                ["_delete"] = new JArray(delete ?? []),
                ["_create"] = new JArray(create ?? []),
                ["_update"] = new JArray(update ?? [])
            };
        }

        private static JObject IterationElementUpdate(Guid iterationIid, Guid elementDefinitionIid)
        {
            return new JObject { ["classKind"] = "Iteration", ["iid"] = iterationIid.ToString(), ["element"] = new JArray(elementDefinitionIid.ToString()) };
        }

        private static JObject ElementDefinition(Guid iid, string shortName, Guid owner, params Guid[] containedElements)
        {
            return new JObject
            {
                ["classKind"] = "ElementDefinition",
                ["iid"] = iid.ToString(),
                ["revisionNumber"] = 0,
                ["shortName"] = shortName,
                ["name"] = shortName,
                ["owner"] = owner.ToString(),
                ["category"] = new JArray(),
                ["hyperLink"] = new JArray(),
                ["definition"] = new JArray(),
                ["alias"] = new JArray(),
                ["referencedElement"] = new JArray(),
                ["parameterGroup"] = new JArray(),
                ["parameter"] = new JArray(),
                ["containedElement"] = new JArray(containedElements.Select(x => x.ToString()))
            };
        }

        private static JObject ElementUsage(Guid iid, string shortName, Guid owner, Guid elementDefinitionIid)
        {
            return new JObject
            {
                ["classKind"] = "ElementUsage",
                ["iid"] = iid.ToString(),
                ["revisionNumber"] = 0,
                ["shortName"] = shortName,
                ["name"] = shortName,
                ["owner"] = owner.ToString(),
                ["elementDefinition"] = elementDefinitionIid.ToString(),
                ["interfaceEnd"] = "NONE",
                ["excludeOption"] = new JArray(),
                ["category"] = new JArray(),
                ["parameterOverride"] = new JArray(),
                ["hyperLink"] = new JArray(),
                ["definition"] = new JArray(),
                ["alias"] = new JArray()
            };
        }

        /// <summary>
        /// The identification of a created IterationSetup and its Iteration
        /// </summary>
        private sealed record IterationSetupInfo(Guid IterationSetupIid, Guid IterationIid, int IterationNumber);
    }
}
