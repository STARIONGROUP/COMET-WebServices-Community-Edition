// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostLifetimeScope.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2026 Starion Group S.A.
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

namespace CometServer.Modules
{
    using System;
    using System.Threading.Tasks;

    using Autofac;

    /// <summary>
    /// The lifetime scope from which the services that handle one POST request are resolved, and who disposes it.
    /// <see cref="ApiBase.HandlePostAsync"/> disposes it when the POST has been handled, unless it has been handed over
    /// to work that continues after the request has ended (<see cref="ApiBase.ProcessPostWithWaitTimeAsync"/>)
    /// </summary>
    public sealed class PostLifetimeScope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostLifetimeScope"/> class
        /// </summary>
        /// <param name="lifetimeScope">
        /// The <see cref="ILifetimeScope"/> of the POST. It must not be a scope of the request, which is disposed when
        /// the request ends
        /// </param>
        internal PostLifetimeScope(ILifetimeScope lifetimeScope)
        {
            this.LifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        /// <summary>
        /// Gets the <see cref="ILifetimeScope"/> from which the services of the POST are resolved
        /// </summary>
        public ILifetimeScope LifetimeScope { get; }

        /// <summary>
        /// Gets a value indicating whether the scope has been handed over, so that whoever took it over disposes it
        /// </summary>
        public bool IsHandedOver { get; private set; }

        /// <summary>
        /// Resolves a service of the POST
        /// </summary>
        /// <typeparam name="T">
        /// The type of the service
        /// </typeparam>
        /// <returns>
        /// The service
        /// </returns>
        public T Resolve<T>() where T : notnull
        {
            return this.LifetimeScope.Resolve<T>();
        }

        /// <summary>
        /// Hands the scope over: from now on the caller disposes it
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// When the scope has already been handed over
        /// </exception>
        internal void HandOver()
        {
            if (this.IsHandedOver)
            {
                throw new InvalidOperationException("The lifetime scope of the POST has already been handed over");
            }

            this.IsHandedOver = true;
        }

        /// <summary>
        /// Disposes the scope, unless it has been handed over
        /// </summary>
        /// <returns>
        /// an awaitable <see cref="ValueTask"/>
        /// </returns>
        internal ValueTask DisposeUnlessHandedOverAsync()
        {
            return this.IsHandedOver ? ValueTask.CompletedTask : this.LifetimeScope.DisposeAsync();
        }
    }
}
