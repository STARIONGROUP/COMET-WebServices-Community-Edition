// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangelogSection.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
// 
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski,
//                 Ahmed Abulwafa Ahmed
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

namespace CDP4WebServices.API.ChangeNotification
{
    /// <summary>
    /// Defines data to be used to compose html/text in a structured form for layout purposes
    /// </summary>
    public class ChangelogSection
    {
        /// <summary>
        /// The title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// The subtitle
        /// </summary>
        public string SubTitle { get; private set; }

        /// <summary>
        /// The description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="ChangelogSection"/> class
        /// </summary>
        /// <param name="title">The title</param>
        /// <param name="subTitle">The subtitle </param>
        /// <param name="description">The description</param>
        public ChangelogSection(string title, string subTitle, string description)
        {
            this.Title = title;
            this.SubTitle = subTitle;
            this.Description = description;
        }
    }
}
