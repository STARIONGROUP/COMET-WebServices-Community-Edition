// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileUploadRequestBinder.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// <remarks>
// Based on source http://bytefish.de/blog/file_upload_nancy/
// </remarks>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.FileHandling
{
    using System;
    using System.Linq;

    using Nancy;
    using Nancy.ModelBinding;

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
        public object Bind(NancyContext context, Type modelType, object instance, BindingConfig configuration, params string[] blackList)
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
        private HttpFile GetFileByKey(NancyContext context, string key)
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
        private long GetContentSize(NancyContext context)
        {
            return context.Request.Headers.ContentLength;
        }
    }
}
