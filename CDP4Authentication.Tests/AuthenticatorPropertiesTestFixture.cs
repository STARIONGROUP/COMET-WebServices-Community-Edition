namespace CDP4Authentication.Tests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    [TestFixture]
    public class AuthenticatorPropertiesTestFixture
    {
        [Test]
        public void AuthenticatorProperties_ShouldStoreAssignedValues()
        {
            var properties = new AuthenticatorProperties
            {
                Rank = 1,
                IsEnabled = true,
                Name = "TestAuthenticator",
                Description = "Authenticator used for unit testing"
            };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(properties.Rank, Is.EqualTo(1));
                Assert.That(properties.IsEnabled, Is.True);
                Assert.That(properties.Name, Is.EqualTo("TestAuthenticator"));
                Assert.That(properties.Description, Is.EqualTo("Authenticator used for unit testing"));
            }
        }

        [Test]
        public void AuthenticationPerson_ShouldInitializeRequiredProperties()
        {
            var iid = Guid.NewGuid();
            const int revision = 5;

            var person = new AuthenticationPerson(iid, revision);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(person.Iid, Is.EqualTo(iid));
                Assert.That(person.RevisionNumber, Is.EqualTo(revision));
                Assert.That(person.IsActive, Is.False);
                Assert.That(person.IsDeprecated, Is.False);
                Assert.That(person.Password, Is.Null);
                Assert.That(person.UserName, Is.Null);
                Assert.That(person.Salt, Is.Null);
                Assert.That(person.Role, Is.Null);
                Assert.That(person.DefaultDomain, Is.Null);
                Assert.That(person.Organization, Is.Null);
            }
        }

        [Test]
        public void AuthenticationPerson_ShouldAllowUpdatingOptionalProperties()
        {
            var iid = Guid.NewGuid();
            const int revision = 10;

            var person = new AuthenticationPerson(iid, revision)
            {
                Password = "password",
                UserName = "user",
                IsActive = true,
                IsDeprecated = true,
                Salt = "salt",
                Role = Guid.NewGuid(),
                DefaultDomain = Guid.NewGuid(),
                Organization = Guid.NewGuid()
            };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(person.Password, Is.EqualTo("password"));
                Assert.That(person.UserName, Is.EqualTo("user"));
                Assert.That(person.IsActive, Is.True);
                Assert.That(person.IsDeprecated, Is.True);
                Assert.That(person.Salt, Is.EqualTo("salt"));
                Assert.That(person.Role, Is.Not.Null);
                Assert.That(person.DefaultDomain, Is.Not.Null);
                Assert.That(person.Organization, Is.Not.Null);
            }
        }

        [Test]
        public void AuthenticatorConfig_ShouldMaintainConnectorProperties()
        {
            var connectorProperties = new AuthenticatorProperties
            {
                Rank = 2,
                IsEnabled = true,
                Name = "Connector",
                Description = "Connector description"
            };

            var config = new AuthenticatorConfig<AuthenticatorProperties>
            {
                AuthenticatorConnectorProperties = new List<AuthenticatorProperties> { connectorProperties }
            };

            Assert.That(config.AuthenticatorConnectorProperties, Is.Not.Null);
            Assert.That(config.AuthenticatorConnectorProperties, Has.Count.EqualTo(1));
            Assert.That(config.AuthenticatorConnectorProperties[0], Is.SameAs(connectorProperties));
        }

        [Test]
        public void AuthenticatorWspProperties_ShouldStoreServerSalts()
        {
            var serverSalts = new[] { "salt1", "salt2" };

            var wspProperties = new AuthenticatorWspProperties
            {
                Rank = 3,
                IsEnabled = false,
                Name = "Wsp",
                Description = "WSP description",
                ServerSalts = serverSalts
            };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(wspProperties.Rank, Is.EqualTo(3));
                Assert.That(wspProperties.IsEnabled, Is.False);
                Assert.That(wspProperties.Name, Is.EqualTo("Wsp"));
                Assert.That(wspProperties.Description, Is.EqualTo("WSP description"));
                Assert.That(wspProperties.ServerSalts, Is.SameAs(serverSalts));
            }
        }
    }
}
