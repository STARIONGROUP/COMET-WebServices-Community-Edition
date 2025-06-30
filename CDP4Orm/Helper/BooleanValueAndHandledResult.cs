// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanAndHandledResult.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Web Services Community Edition. 
//    The CDP4-COMET Web Services Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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
//    Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Helper
{
    /// <summary>
    /// A class to represent a boolean result and an IsHandled indication, typically used for async calls that extect a result and an indication that a call to an "event" (like BeforeAdd, or AfterRead) has been handled.
    /// </summary>
    public struct BooleanValueAndHandledResult
    {
        /// <summary>
        /// The default instance of <see cref="BooleanValueAndHandledResult"/> with a value of true and IsHandled set to false.
        /// </summary>
        public static BooleanValueAndHandledResult Default = new(true, false);

        /// <summary>
        /// Gets a value indicating the result of the "event"
        /// </summary>
        public bool Value { get; }

        /// <summary>
        /// Gets a value indicating if the "event" has been handled, so execution of other handlers can be skipped.
        /// </summary>
        public bool IsHandled { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="BooleanValueAndHandledResult"/> class with the specified value and handled state.
        /// </summary>
        /// <param name="value">The value indicating the result of the "event"</param>
        /// <param name="isHandled">The value indicating if the "event" has been handled, so execution of other handlers can be skipped.</param>
        public BooleanValueAndHandledResult(bool value = true, bool isHandled = false)
        {
            this.Value = value;
            this.IsHandled = isHandled;
        }
    }
}
