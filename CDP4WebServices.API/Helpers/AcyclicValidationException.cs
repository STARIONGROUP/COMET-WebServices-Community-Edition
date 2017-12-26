// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AcyclicValidationException.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Helpers
{
    using System;

    /// <summary>
    /// The CDP4 acyclic validation exception.
    /// </summary>
    public class AcyclicValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AcyclicValidationException"/> class.
        /// </summary>
        public AcyclicValidationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AcyclicValidationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The exception message
        /// </param>
        public AcyclicValidationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AcyclicValidationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The exception message
        /// </param>
        /// <param name="innerException">
        /// A reference to the inner <see cref="Exception"/>
        /// </param>
        public AcyclicValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}