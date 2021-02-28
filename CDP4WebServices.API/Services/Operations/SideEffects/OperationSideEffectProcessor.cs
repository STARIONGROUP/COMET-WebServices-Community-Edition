// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationSideEffectProcessor.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common;
    using CDP4Common.DTO;

    using CometServer.Services.Authorization;

    using NLog;

    using Npgsql;

    /// <summary>
    /// An operation side effect processor class that executes additional logic before and after operations on a
    /// <see cref="Thing" /> instance.
    /// </summary>
    public class OperationSideEffectProcessor : IOperationSideEffectProcessor
    {
        /// <summary>
        /// A <see cref="NLog.Logger" /> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The operation side effect map.
        /// </summary>
        private readonly Dictionary<string, IOperationSideEffect> operationSideEffectMap = new Dictionary<string, IOperationSideEffect>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationSideEffectProcessor" /> class.
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
        /// Gets or sets the <see cref="IRequestUtils" />.
        /// </summary>
        public IRequestUtils RequestUtils { get; set; }

        /// <summary>
        /// Predicate to determine if a property will be validated.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing" /> for which to check if the property validation is to be included.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// True if the passed in property name is to be validated, false to skip validation.
        /// </returns>
        public bool ValidateProperty(Thing thing, string propertyName)
        {
            return this.SideEffects(thing).All(sideEffect => sideEffect.ValidateProperty(thing, propertyName));
        }

        /// <summary>
        /// Execute additional logic before a create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing" /> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing" /> that is inspected.
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
        /// <returns>
        /// Returns a boolean specifying whether the operation shall be executed.
        /// </returns>
        public bool BeforeCreate(Thing thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            return this.SideEffects(thing).All(sideEffect => sideEffect.BeforeCreate(thing, container, transaction, partition, securityContext));
        }

        /// <summary>
        /// Execute additional logic after a successful create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing" /> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing" /> that is inspected.
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
            foreach (var sideEffect in this.SideEffects(thing))
            {
                sideEffect.AfterCreate(thing, container, originalThing, transaction, partition, securityContext);
            }
        }

        /// <summary>
        /// Execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing" /> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing" /> that is inspected.
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
        /// The <see cref="ClasslessDTO" /> instance only contains values for properties that are to be updated.
        /// It is important to note that this variable is not to be changed likely as it can/will change the operation processor
        /// outcome.
        /// </param>
        public void BeforeUpdate(Thing thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ClasslessDTO rawUpdateInfo)
        {
            foreach (var sideEffect in this.SideEffects(thing))
            {
                sideEffect.BeforeUpdate(thing, container, transaction, partition, securityContext, rawUpdateInfo);
            }
        }

        /// <summary>
        /// Execute additional logic after a successful update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing" /> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing" /> that is inspected.
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
            foreach (var sideEffect in this.SideEffects(thing))
            {
                sideEffect.AfterUpdate(thing, container, originalThing, transaction, partition, securityContext);
            }
        }

        /// <summary>
        /// Execute additional logic before a delete operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing" /> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing" /> that is inspected.
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
            foreach (var sideEffect in this.SideEffects(thing))
            {
                sideEffect.BeforeDelete(thing, container, transaction, partition, securityContext);
            }
        }

        /// <summary>
        /// Execute additional logic after a successful delete operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing" /> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing" /> that is inspected.
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
            foreach (var sideEffect in this.SideEffects(thing))
            {
                sideEffect.AfterDelete(thing, container, originalThing, transaction, partition, securityContext);
            }
        }

        /// <summary>
        /// The get operation side effect.
        /// </summary>
        /// <param name="typeName">
        /// The type name of the <see cref="Thing" /> that is being operated upon.
        /// </param>
        /// <returns>
        /// The <see cref="IOperationSideEffect" />.
        /// </returns>
        public IOperationSideEffect GetOperationSideEffect(string typeName)
        {
            return this.operationSideEffectMap[typeName];
        }

        /// <summary>
        /// The get operation side effect.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing" /> that is being operated upon.
        /// </param>
        /// <returns>
        /// The <see cref="IOperationSideEffect" />.
        /// </returns>
        public IOperationSideEffect GetOperationSideEffect(Thing thing)
        {
            return this.operationSideEffectMap[GetTypeName(thing)];
        }

        /// <summary>
        /// The is side effect registered.
        /// </summary>
        /// <param name="typeName">
        /// The type name of the <see cref="Thing" /> that is being operated upon.
        /// </param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public bool IsSideEffectRegistered(string typeName)
        {
            return this.operationSideEffectMap.ContainsKey(typeName);
        }

        /// <summary>
        /// The is side effect registered.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing" /> that is being operated upon.
        /// </param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public bool IsSideEffectRegistered(Thing thing)
        {
            return this.operationSideEffectMap.ContainsKey(GetTypeName(thing));
        }

        /// <summary>
        /// Gets all applicable side effects for the given <paramref name="thing" /> by going up the inheritance chain
        /// and returning the associated <see cref="IOperationSideEffect" /> for each class that has defined one.
        /// Note that this list includes the <see cref="IOperationSideEffect" /> for the given <paramref name="thing" />
        /// as well.
        /// </summary>
        /// <param name="thing">
        /// The given <see cref="Thing" />.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}" /> of all applicable side effects.
        /// </returns>
        private IEnumerable<IOperationSideEffect> SideEffects(Thing thing)
        {
            var type = GetTypeName(thing);

            do
            {
                if (this.IsSideEffectRegistered(type))
                {
                    yield return this.GetOperationSideEffect(type);
                }

                var metaInfo = this.RequestUtils.MetaInfoProvider.GetMetaInfo(type);
                type = metaInfo.BaseType;
            } while (!string.IsNullOrEmpty(type));
        }

        /// <summary>
        /// Utility method to get the type name of a <see cref="Thing" />.
        /// </summary>
        /// <param name="thing">
        /// The thing for which to return the type name.
        /// </param>
        /// <returns>
        /// The type name of a <see cref="Thing" />.
        /// </returns>
        private static string GetTypeName(Thing thing)
        {
            return thing?.GetType().Name;
        }
    }
}
