# CometServer.IntegrationTests

End-to-end tests that run the real CDP4-COMET REST API against a real PostgreSQL database. Use them for behaviour that
unit tests cannot catch: SQL, triggers, audit/revision tables, transaction handling, and full POST/GET round trips.

## How it works

- **NUnit** test project targeting `net10.0` (see `CometServer.IntegrationTests.csproj`).
- `CometServerTestEnvironment` is an NUnit `[SetUpFixture]`, so it runs **once for the whole test run**:
  1. starts a PostgreSQL container with [Testcontainers](https://dotnet.testcontainers.org/) using the image
     `stariongroup/cdp4-test-database-community-edition:4.2.0` (the CI image), with `-c max_locks_per_transaction=1024`,
  2. hosts the CometServer `Startup` **in-process** with ASP.NET Core `TestServer` (Autofac, `Development` environment,
     configuration from `appsettings.IntegrationTests.json` plus the container host/port),
  3. waits for `GET /healthz` to return 200 (timeout 2 minutes),
  4. seeds the database by posting the Annex C.3 file `Data\Data.zip` (linked from `CometServer.Tests\Data\Data.zip`)
     to `POST /Data/Exchange`. This also creates the `cdp4serverrestore` snapshot.
- Every fixture calls `CometServerTestEnvironment.RestoreAsync()` (`POST /Data/Restore`) in its `[SetUp]`, so **every
  test starts from the seeded state**.
- The container is disposed at the end of the run. The tests never touch your local `cdp4server` database, nor a
  database started with `compose.bat` / `docker-compose*.yml`: the container gets a random host port.
- If Docker is not available, the `[OneTimeSetUp]` calls `Assert.Ignore`, so all tests are reported as **ignored**,
  not failed.

## Prerequisites and running

- Docker Desktop (or another Docker engine) running.
- .NET 10 SDK (`net10.0`).

```powershell
# all integration tests (~1.5-2.5 minutes, including container start-up and seeding)
dotnet test CometServer.IntegrationTests\CometServer.IntegrationTests.csproj

# one fixture / one test
dotnet test CometServer.IntegrationTests\CometServer.IntegrationTests.csproj --filter "FullyQualifiedName~IterationSetupCreationTestFixture"
dotnet test CometServer.IntegrationTests\CometServer.IntegrationTests.csproj --filter "FullyQualifiedName~VerifyThatAnIterationCanBeCreatedFromAnOlderIteration"

# all tests of the category, e.g. from the solution
dotnet test COMET-WebServices.sln --filter "Category=Integration"
```

The first run pulls the database image, which takes longer. Server logs are written to the console at `Warning` level
and above (Hangfire at `Error`); lower `MinimumLevel` in `CometServerTestEnvironment` temporarily when debugging.

**CI:** no workflow step targets this project explicitly, but the project is part of `COMET-WebServices.sln`, so the
"Run Unit Tests and Compute Coverage" step of `.github\workflows\CodeQuality.yml` (`dotnet test COMET-WebServices.sln`,
on `ubuntu-22.04`, which has Docker) runs it together with the unit tests.

## Project structure

| File | Purpose |
| --- | --- |
| `CometServerTestEnvironment.cs` | `[SetUpFixture]` for the `CometServer.IntegrationTests` namespace. Starts/stops the container and the in-process server, seeds the data. Public API: `DatabaseImage`, `AdminUserName` (`admin`), `AdminPassword` (`pass`), `DatabaseConnectionString`, `CreateClient(userName, password)`, `RestoreAsync()`. |
| `CometRestClient.cs` | Minimal Annex C.2 JSON client (basic authentication, `Accept: application/json`). `GetAsync(uri)` and `PostAsync(uri, body)` assert HTTP 200 and return a `JArray`; `TryPostAsync(uri, body)` returns `(StatusCode, Content)` without asserting, for negative tests. `IDisposable`. |
| `appsettings.IntegrationTests.json` | Server configuration: basic authentication only, seed/restore enabled, RabbitMQ and changelog disabled. `Backtier:HostName`/`Port` and the `Midtier` directories are overridden at runtime (a temp folder per run). |
| `EngineeringModel\IterationContent.cs` | Wraps the `JArray` of a deep iteration GET: `Iids`, `Things`, `Iteration`, `Get(iid)`, `Contains(iid)`, and `DifferencesWith(expected)`, which compares two iterations ignoring `revisionNumber`, `modifiedOn`, the iteration identity and the Publications (not carried over to a new iteration), with sorted unordered collections. |
| `EngineeringModel\IterationSetupCreationTestFixture.cs` | Tests for creating an IterationSetup/Iteration from the last or an older iteration (audit replay), including multi-user scenarios. Good reference for REST + database assertions. |
| `CometServer.IntegrationTests.csproj` | References `CometServer`; builds `CDP4DatabaseAuthentication` **without** referencing it and copies the DLL + `config.json` to `Authentication\CDP4Database` (mirrors `CopyAuthenticationPlugins` of `CometServer.csproj`); links `Data.zip`. |

## Writing a new test

### 1. Pick a place

One folder per area, matching the namespace: `EngineeringModel\` (things posted to an iteration), `SiteDirectory\`
(persons, models, RDLs, ...). The namespace **must** be `CometServer.IntegrationTests` or a sub-namespace, otherwise the
`[SetUpFixture]` does not run and the server is not started. Name the file `<Subject>TestFixture.cs`.

### 2. Fixture skeleton

Copy the AGPL license header from an existing file (adapt the `file=` attribute). Tests run sequentially against one
shared database: do not add `[Parallelizable]`.

```csharp
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementDefinitionRenameTestFixture.cs" company="Starion Group S.A.">
//    ... AGPL header copied from an existing file ...
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.IntegrationTests.EngineeringModel
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    using Npgsql;

    using NUnit.Framework;

    [TestFixture]
    [Category("Integration")]
    public class ElementDefinitionRenameTestFixture
    {
        private static readonly Guid EngineeringModelIid = Guid.Parse("9ec982e4-ef72-4953-aa85-b158a95d8d56");
        private static readonly Guid FirstIterationIid = Guid.Parse("e163c5ad-f32b-4387-b805-f4b34600bc2c");
        private static readonly Guid TestTwoElementDefinitionIid = Guid.Parse("fe9295c5-af99-494e-86ff-e715837806ae");

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
        public async Task VerifyThatAnElementDefinitionCanBeRenamed()
        {
            var iterationUri = $"EngineeringModel/{EngineeringModelIid}/iteration/{FirstIterationIid}";

            await this.admin.PostAsync(iterationUri, new JObject
            {
                ["_delete"] = new JArray(),
                ["_create"] = new JArray(),
                ["_update"] = new JArray(new JObject
                {
                    ["classKind"] = "ElementDefinition",
                    ["iid"] = TestTwoElementDefinitionIid.ToString(),
                    ["name"] = "Renamed"
                })
            });

            var iteration = new IterationContent(await this.admin.GetAsync($"{iterationUri}?extent=deep"));

            Assert.That(iteration.Get(TestTwoElementDefinitionIid)["name"].Value<string>(), Is.EqualTo("Renamed"));

            // "name" is declared on DefinedThing, so its audit rows are in DefinedThing_Audit; an update ("U") row holds
            // the values from before the update
            var schema = $"Iteration_{EngineeringModelIid.ToString().Replace("-", "_")}";

            await using var connection = new NpgsqlConnection(CometServerTestEnvironment.DatabaseConnectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(
                $"SELECT count(*) FROM \"{schema}\".\"DefinedThing_Audit\" WHERE \"Iid\" = @iid AND \"Action\" = 'U' AND \"ValueTypeDictionary\" -> 'Name' = 'Test 2 Element Definition';",
                connection);

            command.Parameters.AddWithValue("iid", TestTwoElementDefinitionIid);

            Assert.That((long)await command.ExecuteScalarAsync(), Is.GreaterThan(0));
        }
    }
}
```

To act as another user, create a Person + Participant first (see `CreateParticipantAsync` below) and use
`CometServerTestEnvironment.CreateClient(shortName, password)`. Dispose every client you create (`using`).

### 3. POST bodies (Annex C.2)

- A body always has `_delete`, `_create` and `_update` arrays (see the `PostBody(delete, create, update)` helper).
- Every item has `classKind` and `iid`. `_delete` items need nothing else; `_update` items are sparse (only the changed
  properties); `_create` items need **all** mandatory properties, including empty arrays for collections (see the
  `ElementDefinition(...)` / `ElementUsage(...)` helpers for complete examples).
- A created thing must be added to its container in the same POST: `_update` the container's containment property
  (e.g. `Iteration.element`, `ElementDefinition.containedElement`, `SiteDirectory.person`,
  `EngineeringModelSetup.iterationSetup`). The arrays in an `_update` are **added** to the collection.
- POST is only accepted on `SiteDirectory/{iid}` and `EngineeringModel/{iid}/iteration/{iid}`. There is no POST to
  `EngineeringModel/{iid}`: things of the model that are not in an iteration are posted through an iteration.
- Use `TryPostAsync` when the POST is expected to fail and assert on the status code / message.

### 4. Database assertions

Use `CometServerTestEnvironment.DatabaseConnectionString` (PostgreSQL super user, database `cdp4server`, pooling
disabled) with Npgsql. **Always dispose connections** (`await using`): an open connection blocks the database drop of
the next `RestoreAsync`.

- Schemas (partitions): `SiteDirectory`, `EngineeringModel_<model iid>`, `Iteration_<model iid>`, with the dashes of the
  iid replaced by underscores, e.g. `Iteration_9ec982e4_ef72_4953_aa85_b158a95d8d56`. There is one `Iteration_` schema
  per model, shared by all its iterations.
- Table family per class `X`: `X` (current state), `X_Audit` (append-only history with `Action` `I`/`U`/`D`, `Actor`,
  `ValidFrom`, `ValidTo`), `X_Revision` (`Iid`, `RevisionNumber`, `Instant`, `Actor`, `Jsonb`), `X_Cache` (latest
  `Jsonb`, concrete classes only). Quote identifiers: `"Iteration_..."."ElementDefinition_Audit"`.
- A property lives in the table of the class that **declares** it: a rename of an ElementDefinition is audited in
  `DefinedThing_Audit`, not in `ElementDefinition_Audit`. Value properties are keys of the `ValueTypeDictionary`
  hstore column (`"ValueTypeDictionary" -> 'Name'`).
- Audit rows are written by the `process_timetravel_after` trigger: an `I` row holds the inserted values; a `U` row
  holds the values from **before** the update with `ValidTo` = time of the update; a `D` row holds the last values.
  `Actor` is the person of the transaction.
- Audit rows written by the seed (`Data.zip` import) may have a `NULL` `Actor`; read it as `Guid?`.

## Well-known seed data

From `Data.zip`, as used by `IterationSetupCreationTestFixture`:

| Thing | Iid |
| --- | --- |
| SiteDirectory | `f13de6f8-b03a-46e7-a492-53b2f260f294` |
| SiteReferenceDataLibrary | `c454c687-ba3e-44c4-86bc-44544b2c7880` |
| EngineeringModelSetup | `116f6253-89bb-47d4-aa24-d11d197e43c9` |
| EngineeringModel | `9ec982e4-ef72-4953-aa85-b158a95d8d56` |
| IterationSetup (first) | `86163b0e-8341-4316-94fc-93ed60ad0dcf` |
| Iteration (first) | `e163c5ad-f32b-4387-b805-f4b34600bc2c` |
| DomainOfExpertise (test domain) | `0e92edde-fdff-41db-9b1d-f2e484f12535` |
| DomainOfExpertise (additional domain) | `eb759723-14b9-49f4-8611-544d037bb764` |
| PersonRole | `2428f4d9-f26d-4112-9d56-1c940748df69` |
| ParticipantRole | `ee3ae5ff-ac5e-4957-bab1-7698fba2a267` |
| ElementDefinition (test) | `f73860b2-12f0-43e4-b8b2-c81862c0a159` |
| ElementDefinition (test 2, name "Test 2 Element Definition") | `fe9295c5-af99-494e-86ff-e715837806ae` |
| ElementUsage (test) | `75399754-ee45-4bca-b033-63e2019870d1` |
| ElementUsage (test 2) | `f95a1580-e533-4185-b520-208615780afe` |

The administrator is `admin` / `pass`; persons created by a test get the password set in their `password` property.

## Reusable helpers

Currently `private` members of `IterationSetupCreationTestFixture`:

| Helper | What it does |
| --- | --- |
| `CreateParticipantAsync(shortName, password)` | Creates a Person and a Participant of the test model (both domains) and returns an authenticated client and the person iid. |
| `CreateIterationSetupAsync(engineeringModelSetupIid, sourceIterationSetupIid)` | Creates an IterationSetup (and Iteration) from a source; returns `IterationSetupInfo(IterationSetupIid, IterationIid, IterationNumber)`. |
| `CreateEmptyEngineeringModelAsync()` | Creates an EngineeringModelSetup + ModelReferenceDataLibrary without source (empty first iteration). |
| `MakeChangesAsync(iterationIid, client)` | Creates an ElementDefinition, renames "test 2" ElementDefinition, deletes "test 2" ElementUsage. |
| `GetIterationAsync(modelIid, iterationIid)` | Deep GET wrapped in an `IterationContent`. |
| `GetAuditTrailAsync(modelIid, classKind, iids)` / `GetCreatorsAsync(modelIid, classKind, iid, openOnly)` | Read `<classKind>_Audit` of the iteration partition (all entries as `Iid\|Action\|ValidFrom\|ValidTo\|Actor`, with the `ValidTo` of `I` rows left out as `*`; or the `Actor`s of the `I` rows, oldest first, optionally only the open ones). |
| `GetPersonIidAsync(shortName)`, `CountRevisionsAsync(modelIid, classKind, iid, revisionNumber)` | Read the iid of a person; count the `<classKind>_Revision` rows of a thing for one revision number. |
| `AssertSingleOpenInsertAuditEntryAsync(modelIid)` | Fails when any thing of the model has more than one open (`ValidTo = infinity`) `I` audit row, which the read queries join to resolve the `Actor`. |
| `AssertEquivalent(actual, expected)`, `ToSortedJson(things)` | Compare iterations (normalized, or exactly including revision numbers). |
| `PostBody(...)`, `IterationUri(...)`, `IterationElementUpdate(...)`, `ElementDefinition(...)`, `ElementUsage(...)` | Build URIs and JSON bodies. |

If a second fixture needs any of these, move them into a shared helper or base class (e.g. a `SeedData` class with the
iids and a `PostBodies` / `DatabaseAssertions` helper) instead of copying them.

## Tips and pitfalls

- Tests must be independent: the database is restored before each test, so never rely on data or order of another test.
- Use `Assert.Multiple(() => ...)` for synchronous groups and `await Assert.MultipleAsync(async () => ...)` when the
  assertions await (an `async` lambda passed to `Assert.Multiple` is not awaited).
- Publications, and Relationships that relate a Publication, are not carried over to a new iteration;
  `IterationContent` excludes them when comparing iterations.
- `GetAsync` / `PostAsync` already assert HTTP 200 and include the response body in the failure message.
- Prove that a regression test detects the bug: temporarily revert/disable the fix, run the test and check it fails for
  the expected reason, then restore the fix and check it passes.
- These tests complement, not replace, the external
  [ecss-10-25-annexc-integration-tests](https://github.com/STARIONGROUP/ecss-10-25-annexc-integration-tests) suite.

## Code conventions

Same as the rest of the repository (`.github\CONTRIBUTING.md`): 4-space indentation, `using` directives inside the
namespace, always `this.` for instance members, braces on every block, no `#region`, `var` and C# type aliases, long
descriptive names, XML doc comments on helpers.
