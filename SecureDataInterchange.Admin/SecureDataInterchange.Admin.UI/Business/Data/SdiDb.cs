using System;
using System.Data;
using System.Data.SqlClient;

namespace Snsc.FileTransfers.SecureDataInterchange.Business.Data
{
    /// <summary>
    /// Enumeration of available database targets for connection string retrieval.
    /// </summary>
    public enum DatabaseTarget
    {
        /// <summary>
        /// The SecureShare core database (db:core in SNConfig).
        /// </summary>
        Core,
        /// <summary>
        /// The LMS System database for user authentication (db:lmssystem in SNConfig).
        /// </summary>
        LmsSystem
    }

    /// <summary>
    /// Centralized database connection management.
    /// All database connection string retrieval should go through this class.
    /// </summary>
    public static class SdiDb
    {
        /// <summary>
        /// Creates and opens a connection to the specified database.
        /// </summary>
        /// <param name="database">The database to connect to.</param>
        /// <returns>An open IDbConnection instance.</returns>
        public static IDbConnection CreateOpenConnection(DatabaseTarget database)
        {
            string connectionString = GetConnectionString(database);
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Gets the connection string for the specified database.
        /// </summary>
        /// <param name="database">The database to get the connection string for.</param>
        /// <returns>The connection string.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the connection string is not found in SNConfig.</exception>
        public static string GetConnectionString(DatabaseTarget database)
        {
            string key = "db:" + database.ToString().ToLower();
            string databaseName = database.ToString();

            if (Snsc.Configuration.ConfigurationManager.DBConnectionStrings.ContainsKey(key))
            {
                string connectionString = Snsc.Configuration.ConfigurationManager.DBConnectionStrings[key];
                if (!string.IsNullOrEmpty(connectionString) && connectionString.Trim().Length > 0)
                {
                    return connectionString;
                }
            }

            throw new InvalidOperationException(
                string.Format("Database connection string for '{0}' not found in SNConfig. " +
                    "Ensure the SNConfig service has a DBConnectionString entry with ApplicationType='DBConnectionString' and Key='{1}'. " +
                    "Verify that SNConfigWebService and EnvironmentName appSettings are configured correctly in web.config.",
                    databaseName, key));
        }
    }
}
