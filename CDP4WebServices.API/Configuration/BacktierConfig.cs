// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BacktierConfig.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Configuration
{
    /// <summary>
    /// The back tier configuration.
    /// </summary>
    public class BacktierConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BacktierConfig"/> class.
        /// </summary>
        public BacktierConfig()
        {
            // set defaults
            this.HostName = "127.0.0.1";
            this.Port = 5432;
            this.UserName = "cdp4";
            this.Password = "cdp4";
            this.Database = "cdp4server";
            this.DatabaseRestore = "cdp4serverrestore";
            this.DatabaseManage = "cdp4manage";
            this.StatementTimeout = 120;
            this.LogSqlCommands = false;
            this.IsDbSeedEnabled = false;
            this.IsDbRestoreEnabled = false;
        }

        /// <summary>
        /// Gets or sets the host name of the back tier.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the listen port of the back tier.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user password of the back tier.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the database name to be used by the back tier.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the database name to be used by the back tier for a restore database.
        /// </summary>
        public string DatabaseRestore { get; set; }

        /// <summary>
        /// Gets or sets the database name to be used by the back tier for a manage database to control other databases from.
        /// </summary>
        public string DatabaseManage { get; set; }

        /// <summary>
        /// Gets or sets the SQL statement timeout in seconds to observer before an error is thrown.
        /// </summary>
        /// <remarks>
        /// Default is 120 seconds
        /// </remarks>
        public int StatementTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SQL commands are logged
        /// </summary>
        public bool LogSqlCommands { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether database seed is enabled.
        /// </summary>
        public bool IsDbSeedEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether database restore is enabled.
        /// </summary>
        public bool IsDbRestoreEnabled { get; set; }
    }
}
