// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationSideEffectProcessor.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4WebServices.API.Services.Authorization;
    using NLog;
    using Npgsql;

    /// <summary>
    /// An operation side effect processor class that executes additional logic before and after operations on a <see cref="Thing"/> instance.
    /// </summary>
    public class OperationSideEffectProcessor : IOperationSideEffectProcessor
    {
        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The operation side effect map.
        /// </summary>
        private readonly Dictionary<string, IOperationSideEffect> operationSideEffectMap = new Dictionary<string, IOperationSideEffect>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationSideEffectProcessor"/> class. 
        /// </summary>
        /// <param name="operationSideEffects">
        /// The injected operation side effects.
        /// </param>
        /// <remarks>
        /// The injected operation side effects are registered in the operationSideEffectMap.
        /// </remarks>
        public OperationSideEffectProcessor(IEnumerable<IOperationSideEffect> operationSideEffects)
        {
            try
            {
                Logger.Debug("Construct OperationSideEffectProcessor");

                foreach (var operationSideEffect in operationSideEffects)
                {
                    this.operationSideEffectMap.Add(operationSideEffect.RegistryKey, operationSideEffect);
                }

                Logger.Debug("Finished OperationSideEffectProcessor");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// The get operation side effect.
        /// </summary>
        /// <param name="typeName">
        /// The type name of the <see cref="Thing"/> that is being operated upon.
        /// </param>
        /// <returns>
        /// The <see cref="IOperationSideEffect"/>.
        /// </returns>
        public IOperationSideEffect GetOperationSideEffect(string typeName)
        {
            return this.operationSideEffectMap[typeName];
        }

        /// <summary>
        /// The get operation side effect.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> that is being operated upon.
        /// </param>
        /// <returns>
        /// The <see cref="IOperationSideEffect"/>.
        /// </returns>
        public IOperationSideEffect GetOperationSideEffect(Thing thing)
        {
            return this.operationSideEffectMap[this.GetTypeName(thing)];
        }

        /// <summary>
        /// The is side effect registered.
        /// </summary>
        /// <param name="typeName">
        /// The type name of the <see cref="Thing"/> that is being operated upon.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsSideEffectRegistered(string typeName)
        {
            return this.operationSideEffectMap.ContainsKey(typeName);
        }

        /// <summary>
        /// The is side effect registered.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> that is being operated upon.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsSideEffectRegistered(Thing thing)
        {
            return this.operationSideEffectMap.ContainsKey(this.GetTypeName(thing));
        }

        /// <summary>
        /// Predicate to determine if a property will be validated.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> for which to check if the property validation is to be included.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// true if the passed in property name is to be validated,
        /// false to skip validation
        /// </returns>
        public bool ValidateProperty(Thing thing, string propertyName)
        {
            return !this.IsSideEffectRegistered(thing) || this.GetOperationSideEffect(thing).ValidateProperty(thing, propertyName);
        }

        /// <summary>
        /// Execute additional logic before a create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public void BeforeCreate(Thing thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            if (this.IsSideEffectRegistered(thing))
            {
                this.GetOperationSideEffect(thing).BeforeCreate(thing, container, transaction, partition, securityContext);
            }
        }

        /// <summary>
        /// Execute additional logic after a successful create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public void AfterCreate(Thing thing, Thing container, Thing originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            if (this.IsSideEffectRegistered(thing))
            {
                this.GetOperationSideEffect(thing).AfterCreate(thing, container, originalThing, transaction, partition, securityContext);
            }
        }

        /// <summary>
        /// Execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <param name="rawUpdateInfo">
        /// The raw update info that was serialized from the user posted request.
        /// The <see cref="ClasslessDTO"/> instance only contains values for properties that are to be updated.
        /// It is important to note that this variable is not to be changed likely as it can/will change the operation processor outcome.
        /// </param>
        public void BeforeUpdate(Thing thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ClasslessDTO rawUpdateInfo)
        {
            if (this.IsSideEffectRegistered(thing))
            {
                this.GetOperationSideEffect(thing).BeforeUpdate(thing, container, transaction, partition, securityContext, rawUpdateInfo);
            }
        }

        /// <summary>
        /// Execute additional logic after a successful update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public void AfterUpdate(Thing thing, Thing container, Thing originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            if (this.IsSideEffectRegistered(thing))
            {
                this.GetOperationSideEffect(thing).AfterUpdate(thing, container, originalThing, transaction, partition, securityContext);
            }
        }

        /// <summary>
        /// Execute additional logic before a delete operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public void BeforeDelete(Thing thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            if (this.IsSideEffectRegistered(thing))
            {
                this.GetOperationSideEffect(thing).BeforeDelete(thing, container, transaction, partition, securityContext);
            }
        }

        /// <summary>
        /// Execute additional logic after a successful delete operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public void AfterDelete(Thing thing, Thing container, Thing originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            if (this.IsSideEffectRegistered(thing))
            {
                this.GetOperationSideEffect(thing).AfterDelete(thing, container, originalThing, transaction, partition, securityContext);
            }
        }

        /// <summary>
        /// Utility method to get the type name of a <see cref="Thing"/>.
        /// </summary>
        /// <param name="thing">
        /// The thing for which to return the type name.
        /// </param>
        /// <returns>
        /// The type name of a <see cref="Thing"/>.
        /// </returns>
        private string GetTypeName(Thing thing)
        {
            return thing != null ? thing.GetType().Name : null;
        }
    }
}
