// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SavedUserPreference.cs" company="Starion Group S.A.">
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

namespace CometServer.ChangeNotification.UserPreference
{
    using System.Text.Json.Serialization;

    using CDP4Common.SiteDirectoryData;

    /// <summary>
    /// Holds data that can be used to store the <see cref="UserPreference.Value"/> property in a <see cref="UserPreference"/>.
    /// </summary>
    public class SavedUserPreference : ISavedUserPreference
    {
        /// <summary>
        /// The name to be stored
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description to be stored
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The value to be stored
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Instanciates a new <see cref="SavedUserPreference"/>
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="value">The value</param>
        /// <param name="description">The description</param>
        [JsonConstructor]
        public SavedUserPreference(string name, string value, string description = "")
        {
            this.Name = name;
            this.Value = value;
            this.Description = description;
        }
    }
}
 