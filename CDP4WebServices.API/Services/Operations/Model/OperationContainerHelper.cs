// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationContainerHelper.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// <summary>
//   The operation container helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations
{
    using System;

    using CDP4Common.DTO;
    using CDP4Common.MetaInfo;

    /// <summary>
    /// The operation container status.
    /// </summary>
    public enum ResolveStatus
    {
        /// <summary>
        /// Indicates that the container is a resolved instance.
        /// </summary>
        Resolved,

        /// <summary>
        /// Indicates that the container needs to be resolved prior to usage.
        /// </summary>
        NotResolved
    }

    /// <summary>
    /// The operation container helper.
    /// </summary>
    public class OperationContainerHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationContainerHelper"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        public OperationContainerHelper(Thing container, PropertyMetaInfo propertyInfo)
        {
            this.SetProperties(null, container, propertyInfo, ResolveStatus.Resolved);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationContainerHelper"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        public OperationContainerHelper(Thing container, PropertyMetaInfo propertyInfo, long order)
        {
            this.SetProperties(null, container, propertyInfo,  ResolveStatus.Resolved);
            this.ContainmentSequence = order;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationContainerHelper"/> class.
        /// </summary>
        /// <param name="containerInfo">
        /// The container Info used to resolve .
        /// </param>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        public OperationContainerHelper(Tuple<string, Guid> containerInfo, PropertyMetaInfo propertyInfo)
        {
            this.SetProperties(containerInfo, null, propertyInfo, ResolveStatus.NotResolved);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationContainerHelper"/> class.
        /// </summary>
        /// <param name="containerInfo">
        /// The container Info.
        /// </param>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        public OperationContainerHelper(Tuple<string, Guid> containerInfo, PropertyMetaInfo propertyInfo, long order)
        {
            this.SetProperties(containerInfo, null, propertyInfo, ResolveStatus.NotResolved);
            this.ContainmentSequence = order;
        }

        /// <summary>
        /// Gets or sets the container place holder.
        /// </summary>
        public Tuple<string, Guid> ContainerPlaceHolder { get; set; }

        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        public Thing Container { get; set; }

        /// <summary>
        /// Gets the property info.
        /// </summary>
        public PropertyMetaInfo PropertyInfo { get; private set; }

        /// <summary>
        /// Gets the containment sequence.
        /// </summary>
        public long ContainmentSequence { get; private set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the container instance is resolved.
        /// </summary>
        public ResolveStatus ResolveStatus { get; set; }

        /// <summary>
        /// Set the properties from both overloaded constructors.
        /// </summary>
        /// <param name="containerInfo">
        /// The container Info if applicable.
        /// </param>
        /// <param name="container">
        /// The container if applicable.
        /// </param>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        /// <param name="resolveStatus">
        /// The resolve Status.
        /// </param>
        private void SetProperties(Tuple<string, Guid> containerInfo, Thing container, PropertyMetaInfo propertyInfo, ResolveStatus resolveStatus)
        {
            this.ContainerPlaceHolder = containerInfo;
            this.Container = container;
            this.PropertyInfo = propertyInfo;
            this.ResolveStatus = resolveStatus;
        }
    }
}
