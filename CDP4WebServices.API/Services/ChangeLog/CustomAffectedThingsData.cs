// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomAffectedThingsData.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft.
//
//    This file is part of CDP4 Web Services Community Edition. 
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
//
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.ChangeLog
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    /// <summary>
    /// This class holds data that should be added to a <see cref="LogEntryChangelogItem"/>, and/or <see cref="ModelLogEntry"/>.
    /// </summary>
    public class CustomAffectedThingsData
    {
        /// <summary>
        /// A <see cref="HashSet{T}"/> of type <see cref="Guid"/> that contains <see cref="Thing.Iid"/>s from items that need to be added
        /// to a the <see cref="LogEntryChangelogItem.AffectedReferenceIid"/> and <see cref="ModelLogEntry.AffectedItemIid"/> properties.
        /// </summary>
        public HashSet<Guid> AffectedItemIds { get; } = new ();

        /// <summary>
        /// A <see cref="HashSet{T}"/> of type <see cref="Guid"/> that contains <see cref="Thing.Iid"/>s from items that need to be added
        /// to a the <see cref="LogEntryChangelogItem.AffectedReferenceIid"/> and <see cref="ModelLogEntry.AffectedDomainIid"/> properties.
        /// </summary>
        public HashSet<Guid> AffectedDomainIds { get; } = new ();

        /// <summary>
        /// A <see cref="List{T}"/> of type <see cref="string"/> that holds extra <see cref="LogEntryChangelogItem.ChangeDescription"/> data.
        /// </summary>
        public List<string> ExtraChangeDescriptions { get; } = new ();

        /// <summary>
        /// Add data from another <see cref="CustomAffectedThingsData"/> to this <see cref="CustomAffectedThingsData"/>
        /// </summary>
        /// <param name="customAffectedThingsData">
        /// The other <see cref="CustomAffectedThingsData"/>
        /// </param>
        public void AddFrom(CustomAffectedThingsData customAffectedThingsData)
        {
            foreach (var affectedItemId in customAffectedThingsData.AffectedItemIds)
            {
                this.AffectedItemIds.Add(affectedItemId);
            }

            foreach (var affectedDomainId in customAffectedThingsData.AffectedDomainIds)
            {
                this.AffectedDomainIds.Add(affectedDomainId);
            }

            this.ExtraChangeDescriptions.AddRange(customAffectedThingsData.ExtraChangeDescriptions);
        }
    }
}
