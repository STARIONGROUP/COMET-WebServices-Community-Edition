// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utils.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// <summary>
//   This a utility class for Service functionalities
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Autofac;

    using CDP4Common.Types;

    using CDP4WebServices.API.Configuration;
    using CDP4WebServices.API.Modules;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// A utility class that supplies common functionalities to the Service layer.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// The UTC (zulu) date time serialization format.
        /// </summary>
        public const string DateTimeUtcSerializationFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffff'Z'";

        /// <summary>
        /// The capitalize first letter.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If supplied input is null or empty
        /// </exception>
        public static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("string can't be empty!");
            }

            return string.Format(
                "{0}{1}", input.First().ToString(CultureInfo.InvariantCulture).ToUpper(), input.Substring(1));
        }

        /// <summary>
        /// Get a string and lower the first letter
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FirstLetterToLower(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("string can't be empty!");
            }

            return string.Format(
                "{0}{1}", input.First().ToString(CultureInfo.InvariantCulture).ToLower(), input.Substring(1)); 
        }

        /// <summary>
        /// Parse the identifier input as GUID.
        /// </summary>
        /// <param name="input">
        /// The input string which is to be parsed.
        /// </param>
        /// <returns>
        /// The parsed GUID identifier
        /// </returns>
        /// <exception cref="Exception">
        /// If the supplied input is not a valid GUID string representation
        /// </exception>
        public static Guid ParseIdentifier(string input)
        {
            Guid identifier;
            if (!Guid.TryParse(input, out identifier))
            {
                throw new Exception("Invalid identifier supplied");
            }

            return identifier;
        }

        /// <summary>
        /// Construct a POSTGRESQL connection string based on the passed in database name.
        /// </summary>
        /// <param name="database">
        /// The name of the database to connect to.
        /// </param>
        /// <returns>
        /// The constructed connection string
        /// </returns>
        public static string GetConnectionString(string database)
        {
            return string.Format(
                "Server={0};Port={1};User Id={2};Password={3};Database={4};Convert Infinity DateTime=true;CommandTimeout={5};",
                /*{0}*/ AppConfig.Current.Backtier.HostName,
                /*{1}*/ AppConfig.Current.Backtier.Port,
                /*{2}*/ AppConfig.Current.Backtier.UserName,
                /*{3}*/ AppConfig.Current.Backtier.Password,
                /*{4}*/ database,
                /*{5}*/ AppConfig.Current.Backtier.StatementTimeout);
        }

        /// <summary>
        /// Extension method that extract the ordered Ids as a list of GUID from the passed in IEnumerable of <see cref="OrderedItem"/>.
        /// </summary>
        /// <param name="orderedList">
        /// The ordered list of Guids.
        /// </param>
        /// <returns>
        /// A list instance with the extracted Guids.
        /// </returns>
        public static IEnumerable<Guid> ToIdList(this IEnumerable<OrderedItem> orderedList)
        {
            // TODO leave only (Guid)x.V in the Select when json deserialisation problem is solved (task T2780 CDP4WEBSERVICES)
            return orderedList.Select(x => x.V.GetType() == typeof(Guid) ? (Guid)x.V : Guid.Parse(x.V.ToString()));
        }

        /// <summary>
        /// Extension method to check that the DTO thing instance is equal to, or a descendant of the supplied generic type T.
        /// </summary>
        /// <typeparam name="T">
        /// Generic type parameter to check against
        /// </typeparam>
        /// <param name="thing">
        /// The Thing instance on which the type check is performed
        /// </param>
        /// <returns>
        /// True if determined equal or descendant type.
        /// </returns>
        public static bool IsSameOrDerivedClass<T>(this Thing thing)
        {
            var potentialBase = typeof(T);
            var potentialDescendant = thing.GetType();
            return potentialDescendant.IsSubclassOf(potentialBase) || potentialDescendant == potentialBase;
        }

        /// <summary>
        /// Extension method to set authenticated credential information to permission service.
        /// </summary>
        /// <param name="module">
        /// The module.
        /// </param>
        internal static void CdpAuthorization(this ApiBase module)
        {
            module.PermissionService.Credentials = module.RequestUtils.Context.AuthenticatedCredentials;
        }

        /// <summary>
        /// A convenience extension method to register all derived types in the respective assembly as a concrete type, interface type pair (by naming convention)
        /// The bound type info is registered for property injection and is exposed as a singleton within the respective container scope.
        /// </summary>
        /// <typeparam name="TParentType">
        /// The parent type for which to register all derived concepts
        /// </typeparam>
        /// <param name="builder">
        /// The dependency injection container builder used to register the type for injection.
        /// </param>
        internal static void RegisterDerivedTypesAsPropertyInjectedSingleton<TParentType>(this ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(TParentType).Assembly)
                        .Where(x => typeof(TParentType).IsAssignableFrom(x))
                        .AsImplementedInterfaces()
                        .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                        .InstancePerLifetimeScope();
        }
    }
}
