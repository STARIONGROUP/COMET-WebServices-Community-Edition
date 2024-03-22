// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICometTaskService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Tasks
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CDP4DalCommon.Protocol.Tasks;

    /// <summary>
    /// The purpose of the <see cref="ICometTaskService"/> is to provide access to running <see cref="CometTask"/>s which are created for each
    /// POST request. 
    /// </summary>
    public interface ICometTaskService
    {
        /// <summary>
        /// Queries the <see cref="CometTask"/>s for the provided person
        /// </summary>
        /// <param name="person">
        /// The unique identifier of the <see cref="Person"/>
        /// </param>
        /// <returns>
        /// a readonly list of <see cref="CometTask"/>
        /// </returns>
        IReadOnlyList<CometTask> QueryTasks(Guid person);

        /// <summary>
        /// Queries a specific <see cref="CometTask"/>
        /// </summary>
        /// <param name="identifier">
        /// The unique identifier of the <see cref="CometTask"/>
        /// </param>
        /// <returns>
        /// A <see cref="CometTask"/>, or null of not found
        /// </returns>
        CometTask? QueryTask(Guid identifier);

        /// <summary>
        /// Adds or updates the <see cref="CometTask"/> in the Cache
        /// </summary>
        /// <param name="task">
        /// The <see cref="CometTask"/> that is to be added or updated
        /// </param>
        void AddOrUpdateTask(CometTask task);

        /// <summary>
        /// Updates the <see cref="CometTask"/> with the provided data and add or updates it to the Cache
        /// </summary>
        /// <param name="task">
        /// The <see cref="CometTask"/> that is to be added or updated
        /// </param>
        /// <param name="finishedAt">
        /// The <see cref="DateTime"/> at which the <see cref="CometTask"/> was finished
        /// </param>
        /// <param name="statusKind">
        /// the status of the <see cref="CometTask"/>
        /// </param>
        /// <param name="error">
        /// the error in case the operation failed
        /// </param>
        void AddOrUpdateTask(CometTask task, DateTime finishedAt, StatusKind statusKind, string error);

        /// <summary>
        /// Updates the <see cref="CometTask"/> with the provided data, sets it as successful and finised at Now and add or updates it to the Cache
        /// </summary>
        /// <param name="task">
        /// The <see cref="CometTask"/> that is to be added or updated
        /// </param>
        /// <param name="revision">
        /// the revision number corresponding to the TopContainer revision that was updated
        /// </param>
        void AddOrUpdateSuccessfulTask(CometTask task, int revision);
    }
}
