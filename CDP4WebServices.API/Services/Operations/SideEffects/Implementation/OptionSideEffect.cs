// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CDP4Common.DTO;    
    using CDP4Common.Types;
    using CDP4WebServices.API.Services.Authorization;
    using Npgsql;
    
    /// <summary>
    /// The purpose of the <see cref="OptionSideEffect"/> class is to execute additional logic before and after a specific operation is performed
    /// on an <see cref="CDP4Common.DTO.Option"/>
    /// </summary>
    public sealed class OptionSideEffect : OperationSideEffect<Option>
    {
        /// <summary>
        /// a cache of <see cref="Option"/> dependent <see cref="CDP4Common.DTO.Parameter"/>s that is populated in the context of the current <see cref="OptionSideEffect"/>
        /// </summary>
        private readonly Dictionary<Guid, Parameter> optionDependentParameterCache = new Dictionary<Guid, Parameter>();
        
        /// <summary>
        /// a cache of the <see cref="ParameterValueSet"/> that have been created by the current <see cref="OptionSideEffect"/>
        /// </summary>
        private readonly List<ParameterValueSetCacheItem> createdParameterValuetSetCacheItems = new List<ParameterValueSetCacheItem>();

        /// <summary>
        /// a cache of <see cref="CDP4Common.DTO.ActualFiniteStateList"/>s that is filled in the context of the current <see cref="OptionSideEffect"/>
        /// </summary>
        private IEnumerable<ActualFiniteStateList> actualFiniteStateLists;

        /// <summary>
        /// Gets or sets the <see cref="IParameterService"/> that is used to query the <see cref="Parameter"/>s
        /// </summary>
        public IParameterService ParameterService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideService"/> that is used to query the <see cref="ParameterOverride"/>s
        /// </summary>
        public IParameterOverrideService ParameterOverrideService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionService"/> that is used to query <see cref="ParameterSubscription"/>s
        /// </summary>
        public IParameterSubscriptionService ParameterSubscriptionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionValueSetService"/> that is used to create new <see cref="ParameterSubscriptionValueSet"/>
        /// </summary>
        public IParameterSubscriptionValueSetService ParameterSubscriptionValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IActualFiniteStateListService"/> that is used to query the <see cref="ActualFiniteStateList"/>s
        /// </summary>
        public IActualFiniteStateListService ActualFiniteStateListService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterTypeService"/> that is used to query the <see cref="ParameterType"/>s
        /// </summary>
        public IParameterTypeService ParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterTypeComponentService"/> that is used to query the <see cref="ParameterTypeComponent"/>s
        /// </summary>
        public IParameterTypeComponentService ParameterTypeComponentService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterValueSetService"/> that is used to create <see cref="ParameterValueSet"/>s
        /// </summary>
        public IParameterValueSetService ParameterValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideValueSetService"/> that is used to create <see cref="ParameterOverrideValueSet"/>s
        /// </summary>
        public IParameterOverrideValueSetService ParameterOverrideValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterValueSetFactory"/> that is used to create <see cref="ParameterValueSet"/>s
        /// </summary>
        public IParameterValueSetFactory ParameterValueSetFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideValueSetFactory"/> that is used to create <see cref="ParameterOverrideValueSet"/>
        /// </summary>
        public IParameterOverrideValueSetFactory ParameterOverrideValueSetFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionValueSetFactory"/> that is used to create <see cref="ParameterSubscriptionValueSet"/>
        /// </summary>
        public IParameterSubscriptionValueSetFactory ParameterSubscriptionValueSetFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDefaultValueArrayFactory"/> that is used to create default <see cref="ValueArray{String}"/>
        /// </summary>
        public IDefaultValueArrayFactory DefaultValueArrayFactory { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic after a successful create operation.
        /// </summary>
        /// <param name="option">
        /// The <see cref="Option"/> instance that will be created.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Option"/> that is created.
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
        public override void AfterCreate(Option option, Thing container, Option originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // query all the parametertypes and parametertype components and store them in cache. this cache is used to compute the number of values for a ValueArray
            this.InitializeDefaultValueArrayFactory(transaction, securityContext);

            // query and cache all the option dependent parameters for which new valuesets will need to be created for the option that has just been created.
            this.QueryAndCacheAllOptionDependentParameters(transaction, partition, securityContext);
            
            // for each option dependent parameter, create new ParameterValueSets and cache them such that they can later be referenced by the newly created ParameterOverrideValueSets
            var optionDependentParameters = this.optionDependentParameterCache.Values.ToList();
            foreach (var optionDependentParameter in optionDependentParameters)
            {
                this.CreateAndCacheParameterValueSetsAndCreateParameterSubscriptionValueSets(transaction, partition, option, optionDependentParameter, securityContext);
            }

            // for each option dependent ParameterOverride check whether it references an option depedent Parameter; if this is the case, create a new ParameterOverrideValueSet that references the newly created option
            var parameterOverrideDtos = this.ParameterOverrideService.GetShallow(transaction, partition, null, securityContext).Cast<ParameterOverride>();
            foreach (var parameterOverrideDto in parameterOverrideDtos)
            {
                if (this.optionDependentParameterCache.ContainsKey(parameterOverrideDto.Parameter))
                {
                    this.CreateParameterOverrideValueSetsAndParameterSubscriptionValueSets(transaction, partition, option, parameterOverrideDto, securityContext);
                }
            }            
        }
        
        /// <summary>
        /// Creates <see cref="Option"/> dependent <see cref="ParameterValueSet"/>s that are contained by the <see cref="Parameter"/>
        /// as well as <see cref="ParameterSubscriptionValueSet"/>s for the <see cref="ParameterSubscription"/>s that are contained
        /// by the <paramref name="container"/> <see cref="Parameter"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="option">
        /// The <see cref="Option"/> that is used as actual option for the new <see cref="ParameterValueSet"/>s
        /// </param>
        /// <param name="container">
        /// The <see cref="Parameter"/> that is the container of the new <see cref="ParameterValueSet"/>s and that contains the
        /// <see cref="ParameterSubscription"/>s for which new <see cref="ParameterSubscriptionValueSet"/>s need to be created as well.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        private void CreateAndCacheParameterValueSetsAndCreateParameterSubscriptionValueSets(NpgsqlTransaction transaction, string partition, Option option, Parameter container, ISecurityContext securityContext)
        {
            var defaultValueArray = this.DefaultValueArrayFactory.CreateDefaultValueArray(container.ParameterType);

            // get all the ParameterSubscriptions that are contained by the container Parameter, for each of these subscriptions additional ParameterSubscriptionValueSets will be created as well
            var containerParameterSubscriptions = this.ParameterSubscriptionService.GetShallow(transaction, partition, container.ParameterSubscription, securityContext).Cast<ParameterSubscription>().ToList();

            var actualFiniteStateListIid = container.StateDependence;
            if (actualFiniteStateListIid == null)
            {
                var parameterValueSet = this.ParameterValueSetFactory.CreateWithDefaultValueArray(option.Iid, null, defaultValueArray);
                
                this.ParameterValueSetService.CreateConcept(transaction, partition, parameterValueSet, container);

                // the created ParameterValueSet is cached because it will later be referenced by a potentialy created ParameterOverrideValueSet
                var parameterValuetSetCacheItem = new ParameterValueSetCacheItem(container, parameterValueSet);
                this.createdParameterValuetSetCacheItems.Add(parameterValuetSetCacheItem);
                
                this.CreateParameterSubscriptionValueSets(containerParameterSubscriptions, defaultValueArray, parameterValueSet.Iid, transaction, partition);
                
                return;
            }
            
            this.QueryAndCacheAllActualFiniteStateLists(transaction, partition, securityContext);

            var actualFiniteStateList = this.actualFiniteStateLists.Single(x => x.Iid == actualFiniteStateListIid);
            foreach (var actualStateIid in actualFiniteStateList.ActualState)
            {
                var parameterValueSet = this.ParameterValueSetFactory.CreateWithDefaultValueArray(option.Iid, actualStateIid, defaultValueArray);

                this.ParameterValueSetService.CreateConcept(transaction, partition, parameterValueSet, container);

                var parameterValuetSetCacheItem = new ParameterValueSetCacheItem(container, parameterValueSet);
                this.createdParameterValuetSetCacheItems.Add(parameterValuetSetCacheItem);

                this.CreateParameterSubscriptionValueSets(containerParameterSubscriptions, defaultValueArray, parameterValueSet.Iid, transaction, partition);
            }            
        }

        /// <summary>
        /// Creates <see cref="Option"/> dependent <see cref="ParameterOverrideValueSet"/>s that are contained by the <see cref="ParameterOverride"/> 
        /// as well as <see cref="ParameterSubscriptionValueSet"/>s for the <see cref="ParameterSubscription"/>s that are contained
        /// by the <paramref name="container"/> <see cref="ParameterOverride"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="option">
        /// The <see cref="Option"/> that is uses as actual option for the new <see cref="ParameterOverrideValueSet"/>s
        /// </param>
        /// <param name="container">
        /// The <see cref="ParameterOverride"/> that is the container of the new <see cref="ParameterOverrideValueSet"/>s
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        private void CreateParameterOverrideValueSetsAndParameterSubscriptionValueSets(NpgsqlTransaction transaction, string partition, Option option, ParameterOverride container, ISecurityContext securityContext)
        {
            Parameter parameter;
            if (!this.optionDependentParameterCache.TryGetValue(container.Parameter, out parameter))
            {
                throw new KeyNotFoundException(string.Format("The Parameter with iid {0} could not be found", container.Parameter));
            }

            var defaultValueArray = this.DefaultValueArrayFactory.CreateDefaultValueArray(container.Parameter);

            // get all the ParameterSubscriptions that are contained by the container ParameterOverride, for each of these subscriptions additional ParameterSubscriptionValueSets will be created as well
            var containerParameterSubscriptions = this.ParameterSubscriptionService.GetShallow(transaction, partition, container.ParameterSubscription, securityContext).Cast<ParameterSubscription>().ToList();

            if (parameter.StateDependence == null)
            {
                var parameterValueSet = this.QueryParameterValueSet(parameter.Iid, option.Iid, null);

                var parameterOverrideValueSet = this.ParameterOverrideValueSetFactory.CreateWithDefaultValueArray(parameterValueSet.Iid, defaultValueArray);
                
                this.ParameterOverrideValueSetService.CreateConcept(transaction, partition, parameterOverrideValueSet, container);

                this.CreateParameterSubscriptionValueSets(containerParameterSubscriptions, defaultValueArray, parameterOverrideValueSet.Iid, transaction, partition);
                
                return;
            }

            this.QueryAndCacheAllActualFiniteStateLists(transaction, partition, securityContext);

            var actualFiniteStateList = this.actualFiniteStateLists.Single(x => x.Iid == parameter.StateDependence);
            foreach (var actualStateIid in actualFiniteStateList.ActualState)
            {
                var parameterValueSet = this.QueryParameterValueSet(parameter.Iid, option.Iid, actualStateIid);

                var parameterOverrideValueSet = this.ParameterOverrideValueSetFactory.CreateWithDefaultValueArray(parameterValueSet.Iid, defaultValueArray);

                this.ParameterOverrideValueSetService.CreateConcept(transaction, partition, parameterOverrideValueSet, container);

                this.CreateParameterSubscriptionValueSets(containerParameterSubscriptions, defaultValueArray, parameterOverrideValueSet.Iid, transaction, partition);
            }
        }

        /// <summary>
        /// Creates a new <see cref="ParameterSubscriptionValueSet"/>
        /// </summary>
        /// <param name="containerParameterSubscriptions">
        /// The <see cref="ParameterSubscription"/>s that are the containers of the <see cref="ParameterSubscriptionValueSet"/>s that are to be created
        /// </param>
        /// <param name="defaultValueArray">
        /// the default <see cref="ValueArray{String}"/> that is used to initialize the manual value of the <see cref="ParameterSubscriptionValueSet"/>s that are to be created
        /// </param>
        /// <param name="subscribedValueSetIid">
        /// The unique id of the <see cref="ParameterValueSetBase"/> that is referenced by the <see cref="ParameterSubscriptionValueSet"/>s that are to be created
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        private void CreateParameterSubscriptionValueSets(IEnumerable<ParameterSubscription> containerParameterSubscriptions, ValueArray<string> defaultValueArray, Guid subscribedValueSetIid, NpgsqlTransaction transaction, string partition)
        {
            foreach (var parameterSubscription in containerParameterSubscriptions)
            {
                var parameterSubscriptionValueSet =
                    this.ParameterSubscriptionValueSetFactory.CreateWithDefaultValueArray(
                        subscribedValueSetIid, 
                        defaultValueArray);
                
                this.ParameterSubscriptionValueSetService.CreateConcept(transaction, partition, parameterSubscriptionValueSet, parameterSubscription);
            }
        }

        /// <summary>
        /// Queries the created <see cref="ParameterValueSet"/> from the createdParameterValueSetsCache data store
        /// </summary>
        /// <param name="parameterIid">
        /// The unique id of the <see cref="Parameter"/> that is the container of the <see cref="ParameterValueSet"/> that is queried
        /// </param>
        /// <param name="optionIid">
        /// The unique Id of the <see cref="Option"/> that is the actual <see cref="Option"/> of the queried <see cref="ParameterValueSet"/>
        /// </param>
        /// <param name="actualStateIid">
        /// The unique Id of the <see cref="CDP4Common.DTO.ActualFiniteState"/> that is the actual state of the queried <see cref="ParameterValueSet"/>
        /// </param>
        /// <returns>
        /// The requested cached <see cref="ParameterValueSet"/>
        /// </returns>
        private ParameterValueSet QueryParameterValueSet(Guid parameterIid, Guid optionIid, Guid? actualStateIid)
        {
            var parameterValuetSetCacheItem = this.createdParameterValuetSetCacheItems.SingleOrDefault(x => x.Parameter.Iid == parameterIid && x.ParameterValueSet.ActualOption == optionIid && x.ParameterValueSet.ActualState == actualStateIid);
            if (parameterValuetSetCacheItem == null)
            {
                throw new KeyNotFoundException(string.Format("The created ParameterValueSet with ActualOption {0} and actualStateIid {1} could not be found", optionIid, actualStateIid));
            }

            return parameterValuetSetCacheItem.ParameterValueSet;
        }

        /// <summary>
        /// Queries all the <see cref="ActualFiniteStateList"/> DTO's and caches them in the <see cref="actualFiniteStateLists"/> private field
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be retrieved from.
        /// </param>
        /// <param name="securityContext">
        /// The <see cref="ISecurityContext"/> used for permission checking.
        /// </param>
        private void QueryAndCacheAllActualFiniteStateLists(NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            if (this.actualFiniteStateLists == null)
            {
                this.actualFiniteStateLists = this.ActualFiniteStateListService.GetShallow(transaction, partition, null, securityContext).Cast<ActualFiniteStateList>();
            }
        }

        /// <summary>
        /// Queries the data store for all the <see cref="ParameterType"/>s and <see cref="ParameterTypeComponent"/>s and populates
        /// the parameterTypeCache and parameterTypeComponentCache data stores
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="securityContext">
        /// The <see cref="ISecurityContext"/> used for permission checking.
        /// </param>
        private void InitializeDefaultValueArrayFactory(NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            var parameterTypes = this.ParameterTypeService.GetShallow(transaction, CDP4Orm.Dao.Utils.SiteDirectoryPartition, null, securityContext).Cast<ParameterType>();
            var parameterTypeComponents = this.ParameterTypeComponentService.GetShallow(transaction, CDP4Orm.Dao.Utils.SiteDirectoryPartition, null, securityContext).Cast<ParameterTypeComponent>();

            this.DefaultValueArrayFactory.Initialize(parameterTypes, parameterTypeComponents);
        }

        /// <summary>
        /// Queries the data store for all the <see cref="Parameter"/>s and populates the <see cref="optionDependentParameterCache"/> 
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be retrieved from.
        /// </param>
        /// <param name="securityContext">
        /// The <see cref="ISecurityContext"/> used for permission checking.
        /// </param>
        private void QueryAndCacheAllOptionDependentParameters(NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // TODO: it may be a good idea to create an extra function in the ParameterDoa to retrieve the optiondependent parameters (task T2819 CDP4WEBSERVICES)
            if (this.optionDependentParameterCache.Count != 0)
            {
                return;
            }

            var parameters = this.ParameterService.GetShallow(transaction, partition, null, securityContext).Cast<Parameter>();
            foreach (var parameter in parameters)
            {
                if (parameter.IsOptionDependent)
                {
                    this.optionDependentParameterCache.Add(parameter.Iid, parameter);
                }                    
            }
        }

        /// <summary>
        /// The parameter value set cache item class.
        /// </summary>
        private class ParameterValueSetCacheItem
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ParameterValueSetCacheItem"/> class. 
            /// </summary>
            /// <param name="parameter">
            /// The <see cref="Parameter"/> to which the <see cref="ParameterValueSet"/> is associated
            /// </param>
            /// <param name="parameterValueSet">
            /// The <see cref="ParameterValueSet"/> that is associated to the <see cref="Parameter"/>
            /// </param>
            public ParameterValueSetCacheItem(Parameter parameter, ParameterValueSet parameterValueSet)
            {
                this.Parameter = parameter;
                this.ParameterValueSet = parameterValueSet;
            }

            /// <summary>
            /// Gets the parameter.
            /// </summary>
            public Parameter Parameter { get; private set; }

            /// <summary>
            /// Gets the parameter value set.
            /// </summary>
            public ParameterValueSet ParameterValueSet { get; private set; }
        }
    }
}