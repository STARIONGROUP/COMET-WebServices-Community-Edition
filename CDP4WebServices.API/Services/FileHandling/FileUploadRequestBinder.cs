// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileUploadRequestBinder.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.FileHandling
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// The file upload request binder.
    /// </summary>
    public class FileUploadRequestBinder : IModelBinder
    {
        /// <summary>
        /// The bind.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="modelType">
        /// The model type.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <param name="blackList">
        /// The black list.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Bind(HttpContext context, Type modelType, object instance, BindingConfig configuration, params string[] blackList)
        {
            var exchangeFileUpload = (instance as ExchangeFileUploadRequest) ?? new ExchangeFileUploadRequest();
            
            exchangeFileUpload.Password = context.Request.Form["password"];
            exchangeFileUpload.File = this.GetFileByKey(context, "file");
            exchangeFileUpload.ContentSize = this.GetContentSize(context);

            return exchangeFileUpload;
        }

        /// <summary>
        /// The can bind.
        /// </summary>
        /// <param name="modelType">
        /// The model type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanBind(Type modelType)
        {
            return modelType == typeof(ExchangeFileUploadRequest);
        }

        /// <summary>
        /// The get file by key.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="HttpFile"/>.
        /// </returns>
        private HttpFile GetFileByKey(HttpContext context, string key)
        {
            var files = context.Request.Files;
            return files != null ? files.FirstOrDefault(x => x.Key == key) : null;
        }

        /// <summary>
        /// The get content size.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The size of the content <see cref="long"/>.
        /// </returns>
        private long GetContentSize(HttpContext context)
        {
            return context.Request.Headers.ContentLength;
        }
    }
}
