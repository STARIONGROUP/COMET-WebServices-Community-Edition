// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System;
    using CDP4Common.DTO;
    using Npgsql;
    using NpgsqlTypes;

    /// <summary>
    /// The Iteration Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class IterationDao
    {
        /// <summary>
        /// Execute additional logic after each write function call.
        /// </summary>
        /// <param name="writeResult">
        /// The result from the main write logic.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The thing DTO that was persisted.
        /// </param>
        /// <param name="container">
        /// The container of the persisted DTO.
        /// </param>
        /// <returns>
        /// True if the additional logic was successfully executed.
        /// </returns>
        public new bool AfterWrite(bool writeResult, NpgsqlTransaction transaction, string partition, Thing thing, Thing container)
        {
            // move iteration ahead update containment info for all iteration contained concepts
            
            var iterationPartition = string.Format("{0}_{1}", "Iteration", container.Iid.ToString().Replace("-", "_")); 

            this.MoveToNextIteration(transaction, iterationPartition, thing);

            // TODO 0 - relate current transaction revision to new iteration to ensure iteration wide revision time (task T2814 CDP4WEBSERVICES)
            // TODO 1 - for each iteration contained concept: drop timetravel triggers. (task T2814 CDP4WEBSERVICES)
            // TODO 2 - for each iteration contained concept: delete data from 'current' table. (task T2814 CDP4WEBSERVICES)
            // TODO 3 - for each iteration contained concept: insert selection of timestamped data (as set in the temp transaction_info table) into 'current' table. (task T2814 CDP4WEBSERVICES)
            // TODO 3.1 - !!make sure to set the validfrom column to this iteration revision timestamp. (task T2814 CDP4WEBSERVICES)
            // TODO 4 - for each iteration contained concept: reinstate timetravel triggers. (task T2814 CDP4WEBSERVICES)
            return base.AfterWrite(writeResult, transaction, partition, thing, container);
        }

        private void BranchFromSourceIteration(NpgsqlTransaction transaction, string partition, Thing sourceIteration, Thing targetIteration)
        {
            // TODO (task T2814 CDP4WEBSERVICES)
            // clean up current views
            string.Format("DELETE FROM \"{0}\".\"{1}\"", partition, "Thing");

            // insert the data from historic records into table with the container set to the new iteration
            string.Format("INSERT INTO \"{0}\".\"{1}\"", partition, "Option");
            string.Format(" (\"{0}\", \"{1}\", \"Container\")");
            string.Format(" SELECT \"{0}\", \"{1}\", ");
            string.Format(" FROM \"{0}\".\"{1}\"", partition, "Option");
        }

        /// <summary>
        /// update containment information
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="containedType">
        /// The contained Type.
        /// </param>
        /// <param name="sourceIteration">
        /// The source Iteration.
        /// </param>
        /// <param name="newIteration">
        /// The new Iteration.
        /// </param>
        private void BranchContainment(NpgsqlTransaction transaction, string partition, Type containedType, Thing sourceIteration, Thing newIteration)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = string.Format("DELETE FROM \"{0}\".\"{1}\";", partition, containedType.Name);
                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                command.ExecuteNonQuery();
            }
            
            using(var command = new NpgsqlCommand())
            {
                command.CommandText = string.Format("INSERT INTO \"{0}\".\"{1}\" SELECT *, :containerId FROM \"{0}\".\"{1}_Data\" SET \"Container\" = :container;", partition, containedType.Name);
                command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = sourceIteration.Iid;
                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Move the database to the next iteration.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The thing DTO that is to be persisted.
        /// </param>
        private void MoveToNextIteration(NpgsqlTransaction transaction, string partition, Thing thing)
        {
            // TODO code generate (task T2814 CDP4WEBSERVICES)
            this.UpdateContainment(transaction, partition, typeof(Option), thing);
            this.UpdateContainment(transaction, partition, typeof(Publication), thing);
            this.UpdateContainment(transaction, partition, typeof(PossibleFiniteStateList), thing);
            this.UpdateContainment(transaction, partition, typeof(ElementDefinition), thing);
            this.UpdateContainment(transaction, partition, typeof(Relationship), thing);
            this.UpdateContainment(transaction, partition, typeof(ExternalIdentifierMap), thing);
            this.UpdateContainment(transaction, partition, typeof(RequirementsSpecification), thing);
            this.UpdateContainment(transaction, partition, typeof(DomainFileStore), thing);
            this.UpdateContainment(transaction, partition, typeof(ActualFiniteStateList), thing);
            this.UpdateContainment(transaction, partition, typeof(RuleVerificationList), thing);
        }

        /// <summary>
        /// update containment information
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="containedType">
        /// The contained Type.
        /// </param>
        /// <param name="thing">
        /// The thing DTO that is to be persisted.
        /// </param>
        private void UpdateContainment(NpgsqlTransaction transaction, string partition, Type containedType, Thing thing)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = string.Format("UPDATE \"{0}\".\"{1}\" SET \"Container\" = :container;", partition, containedType.Name);
                command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = thing.Iid;
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                this.ExecuteAndLogCommand(command);
            }
        }
    }
}
