// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionBusinessLogicService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
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
