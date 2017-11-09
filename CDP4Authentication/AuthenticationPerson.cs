// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationPerson.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Authentication
{
    using System;

    /// <summary>
    /// The authentication person.
    /// </summary>
    public class AuthenticationPerson
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationPerson"/> class.
        /// </summary>
        /// <param name="iid">
        /// The identifier of the instance.
        /// </param>
        /// <param name="revisionNumber">
        /// The revision number of the instance.
        /// </param>
        public AuthenticationPerson(Guid iid, int revisionNumber)
        {
            this.Iid = iid;
            this.RevisionNumber = revisionNumber;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Guid Iid { get; set; }

        /// <summary>
        /// Gets or sets the revision number.
        /// </summary>
        public int RevisionNumber { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the salt.
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// Gets or sets the Person Role id.
        /// </summary>
        public Guid? Role { get; set; }

        /// <summary>
        /// Gets or sets the default domain.
        /// </summary>
        public Guid? DefaultDomain { get; set; }
    }
}
