// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionBusinessLogicService.cs" company="RHEA System S.A.">
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

namespace CometServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.DTO;

    /// <summary>
    /// The purpose of the <see cref="OptionBusinessLogicService"/> is to provide services for <see cref="Option"/>s instances
    /// </summary>
    public class OptionBusinessLogicService : IOptionBusinessLogicService
    {
        /// <summary>
        /// The current <see cref="Iteration"/>
        /// </summary>
        private Iteration currentIteration;

        /// <summary>
        /// The current <see cref="Option"/>s
        /// </summary>
        private IReadOnlyCollection<Option> currentOptions;

        /// <summary>
        /// Initializes the <see cref="OptionBusinessLogicService"/>
        /// </summary>
        /// <param name="iteration">The current <see cref="Iteration"/></param>
        /// <param name="options">The current <see cref="Option"/>s</param>
        public void Initialize(Iteration iteration, IReadOnlyCollection<Option> options)
        {
            if (iteration == null)
            {
                throw new ArgumentNullException("iteration");
            }

            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            if (iteration.Option.Count != options.Count())
            {
                throw new InvalidOperationException("The number of options does not correspond to the number of options in the iteration.");
            }

            this.currentIteration = iteration;
            this.currentOptions = options;
        }

        /// <summary>
        /// Computes the default <see cref="Option"/> for an iteration
        /// </summary>
        /// <returns>The default <see cref="Option"/> if any</returns>
        public Option GetDefaultOption()
        {
            return this.currentIteration.DefaultOption.HasValue ? this.currentOptions.Single(x => x.Iid == this.currentIteration.DefaultOption.Value) : null;
        }
    }
}
