namespace CDP4WebServices.API.Services
{
    using CDP4Common.DTO;
    using CDP4Orm.Dao;
    using Npgsql;

    /// <summary>
    /// The Person Service Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    partial interface IPersonService
    {
        bool UpdateCredentials(NpgsqlTransaction transaction, string partition, Thing thing, MigrationPasswordCredentials credentials);
    }
}
