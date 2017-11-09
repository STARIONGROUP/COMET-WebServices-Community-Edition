// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cdp4DatabaseDumpException.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Helpers
{
    using System;

    /// <summary>
    /// The CDP 4 database dump exception.
    /// </summary>
    public class Cdp4DatabaseDumpException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cdp4DatabaseDumpException"/> class.
        /// </summary>
        public Cdp4DatabaseDumpException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cdp4DatabaseDumpException"/> class.
        /// </summary>
        /// <param name="message">
        /// The exception message
        /// </param>
        public Cdp4DatabaseDumpException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cdp4DatabaseDumpException"/> class.
        /// </summary>
        /// <param name="message">
        /// The exception message
        /// </param>
        /// <param name="innerException">
        /// A reference to the inner <see cref="Exception"/>
        /// </param>
        public Cdp4DatabaseDumpException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}