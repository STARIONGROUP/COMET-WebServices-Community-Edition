// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonSideEffect.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
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

namespace CometServer.Services.Operations.SideEffects
{
    using System;

    using Authorization;

    using CDP4Common;
    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="PersonSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class PersonSideEffect : OperationSideEffect<Person>
    {
        /// <summary>
        /// The password key.
        /// </summary>
        private const string PasswordKey = "Password";

        /// <summary>
        /// Gets or sets the <see cref="IPersonDao"/>.
        /// </summary>
        public IPersonDao PersonDao { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ICredentialsService"/> used for authorization
        /// </summary>
        public ICredentialsService CredentialsService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <param name="rawUpdateInfo">
        /// The update info that was serialized directly from the user request. 
        /// The raw <see cref="ClasslessDTO"/> instance only contains values for properties that are to be updated.
        /// It is important to note that this variable is not to be edited likely: it can/will change the operation processor outcome.
        /// </param>
        public override void BeforeUpdate(Person thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ClasslessDTO rawUpdateInfo)
        {
            string passwordValue;

            if (TryExtractPasswordUpdate(rawUpdateInfo, out passwordValue))
            {
                // A password change is invoked:
                // encapsulate the new 'clear-text' password with a token to signal the ORM layer that further specific processing is required
                this.EncapsulatePasswordWithChangeToken(passwordValue, rawUpdateInfo);
            }
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic after a successful update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Person"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <remarks>
        /// The person that is executing the request may not update his own role property. 
        /// </remarks>
        public override void AfterUpdate(Person thing, Thing container, Person originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var authenticatedCredentials = this.CredentialsService.Credentials;
            
            if (authenticatedCredentials.Person.Iid == thing.Iid)
            {
                if (thing.Role != originalThing.Role)
                {
                    throw new InvalidOperationException("Update to role of the Person making the request is not allowed");
                }

                if (!thing.IsActive)
                {
                    throw new InvalidOperationException("Update IsActive = false to own Person is not allowed");
                }

                if (thing.IsDeprecated)
                {
                    throw new InvalidOperationException("Update IsDeprecated = true to own Person is not allowed");
                }
            }
        }

        /// <summary>
        /// Extract password value.
        /// </summary>
        /// <param name="rawUpdateInfo">
        /// The raw update info.
        /// </param>
        /// <param name="passwordValue">
        /// The password value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool TryExtractPasswordUpdate(ClasslessDTO rawUpdateInfo, out string passwordValue)
        {
            if (rawUpdateInfo.TryGetValue(PasswordKey, out var value))
            {
                var password = value.ToString();

                if (string.IsNullOrWhiteSpace(password))
                {
                    // this is an invalid password value request, remove this from further processing
                    rawUpdateInfo.Remove(PasswordKey);

                    passwordValue = null;
                    return false;
                }

                passwordValue = password;
                return true;
            }
            
            passwordValue = null;
            return false;
        }

        /// <summary>
        /// Encapsulate the 'clear-text' password value with a password change token that is only valid for this request.
        /// </summary>
        /// <param name="passwordValue">
        /// The password Value.
        /// </param>
        /// <param name="rawUpdateInfo">
        /// The raw update info.
        /// </param>
        private void EncapsulatePasswordWithChangeToken(string passwordValue, ClasslessDTO rawUpdateInfo)
        {
            // Signal the ORM layer that a password change request is being handled:
            // encapsulate the 'clear-text' password with the PasswordChangeToken that is valid for this request only
            var encapsulatedPasswordChangeRequest = $"{this.PersonDao.PasswordChangeToken}{passwordValue}{this.PersonDao.PasswordChangeToken}";

            // override the update info and override the rawUpdateInfo value
            rawUpdateInfo[PasswordKey] = encapsulatedPasswordChangeRequest;
        }
    }
}
