// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CometJsonModelBinder.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft.
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

namespace CometServer
{
    using System.Threading.Tasks;
    
    using CDP4JsonSerializer;
    
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;

    /// <summary>
    /// The purpose of the <see cref="CometJsonModelBinder"/> is to provide model binding for <see cref="HttpRequest"/>
    /// to the <see cref="Cdp4JsonSerializer"/>
    /// </summary>
    public class CometJsonModelBinder : Carter.ModelBinding.IModelBinder
    {
        /// <summary>
        /// The <see cref="MarvinJsonSerializer"/> used for <see cref="HttpRequest"/> deserialization
        /// </summary>
        private static readonly Cdp4JsonSerializer JsonSerializer = new Cdp4JsonSerializer();

        /// <summary>
        /// Bind the <see cref="HttpContext"/> to a Marvin Domain object
        /// </summary>
        /// <typeparam name="T">
        /// The type to bind to
        /// </typeparam>
        /// <param name="request">
        /// The <see cref="HttpRequest"/> that contains the Body that needs to be deserialized
        /// </param>
        /// <returns>
        /// a model instance
        /// </returns>
        public Task<T> Bind<T>(HttpRequest request)
        {
            var syncIOFeature = request.HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }

            return Task.FromResult(JsonSerializer.Deserialize<T>(request.Body));
        }
    }
}
