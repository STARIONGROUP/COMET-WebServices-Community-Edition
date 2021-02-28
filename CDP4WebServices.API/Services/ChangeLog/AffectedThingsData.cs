// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AffectedThingsData.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.ChangeLog
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    /// <summary>
    /// This class holds data that should be added to a <see cref="LogEntryChangelogItem"/>, and/or <see cref="ModelLogEntry"/>.
    /// </summary>
    public class AffectedThingsData
    {
        /// <summary>
        /// A <see cref="HashSet{T}"/> of type <see cref="Guid"/> that contains <see cref="Thing.Iid"/>s from items that need to be added
        /// to a the <see cref="LogEntryChangelogItem.AffectedReferenceIid"/> and <see cref="ModelLogEntry.AffectedItemIid"/> properties.
        /// </summary>
        public HashSet<Guid> AffectedItemIds { get; } = new HashSet<Guid>();

        /// <summary>
        /// A <see cref="HashSet{T}"/> of type <see cref="Guid"/> that contains <see cref="Thing.Iid"/>s from items that need to be added
        /// to a the <see cref="LogEntryChangelogItem.AffectedReferenceIid"/> and <see cref="ModelLogEntry.AffectedDomainIid"/> properties.
        /// </summary>
        public HashSet<Guid> AffectedDomainIds { get; } = new HashSet<Guid>();

        /// <summary>
        /// A <see cref="List{T}"/> of type <see cref="string"/> that holds extra <see cref="LogEntryChangelogItem.ChangeDescription"/> data.
        /// </summary>
        public List<string> ExtraChangeDescriptions { get; } = new List<string>();

        /// <summary>
        /// Add data from another <see cref="AffectedThingsData"/> to this <see cref="AffectedThingsData"/>
        /// </summary>
        /// <param name="affectedThingsData">
        /// The other <see cref="AffectedThingsData"/>
        /// </param>
        public void AddFrom(AffectedThingsData affectedThingsData)
        {
            foreach (var affectedItemId in affectedThingsData.AffectedItemIds)
            {
                this.AffectedItemIds.Add(affectedItemId);
            }

            foreach (var affectedDomainId in affectedThingsData.AffectedDomainIds)
            {
                this.AffectedDomainIds.Add(affectedDomainId);
            }

            this.ExtraChangeDescriptions.AddRange(affectedThingsData.ExtraChangeDescriptions);
        }
    }
}
