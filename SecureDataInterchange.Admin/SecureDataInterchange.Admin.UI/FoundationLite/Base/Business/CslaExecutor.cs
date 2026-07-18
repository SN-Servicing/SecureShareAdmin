using Snsc.Base.Data;
using Snsc.FileTransfers.SecureDataInterchange.Business.Data;
using System;
using System.Configuration;
using System.Data;

namespace Snsc.Base.Business
{
    public enum DatabaseCommandExecutionStyle
    {
        NonQuery,
        DataTable,
        Scalar
    }

    /// <summary>
    /// Replacement for the removed Snsc.Base private package CslaExecutor.
    /// Executes a StoredProcedureExecutorBase as a non-query or fills a DataTable.
    /// </summary>
    public class CslaExecutor
    {
        private StoredProcedureExecutorBase _spew;
        private DatabaseCommandExecutionStyle _style;

        private CslaExecutor(StoredProcedureExecutorBase spew, DatabaseCommandExecutionStyle style)
        {
            _spew = spew;
            _style = style;
        }

        public object ProcedureReturnValue { get; private set; }

        public StoredProcedureExecutorBase StoredProcedureExecObject
        { get { return _spew; } }

        public static CslaExecutor NewCslaExecutor(StoredProcedureExecutorBase spew, DatabaseCommandExecutionStyle style)
        {
            return new CslaExecutor(spew, style);
        }

        public CslaExecutor Execute()
        {
            string connectionString = GetConnectionString(_spew.DatabaseName);
            using (var cn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                cn.Open();
                _spew.SetConnection(cn);
                switch (_style)
                {
                    case DatabaseCommandExecutionStyle.NonQuery:
                        _spew.ExecuteNonQuery();
                        break;

                    case DatabaseCommandExecutionStyle.DataTable:
                        var dt = new DataTable();
                        _spew.Fill(dt);
                        ProcedureReturnValue = dt;
                        break;

                    case DatabaseCommandExecutionStyle.Scalar:
                        ProcedureReturnValue = _spew.ExecuteScalar();
                        break;
                }
            }
            return this;
        }

        private static string GetConnectionString(string dbname)
        {
            // Map common database names to the DatabaseTarget enum
            DatabaseTarget database;
            string dbnameLower = dbname.ToLower();
            
            switch (dbnameLower)
            {
                case "core":
                case "secureshare":
                    database = DatabaseTarget.Core;
                    break;
                case "lmssystem":
                case "lms":
                    database = DatabaseTarget.LmsSystem;
                    break;
                default:
                    // For unknown databases, try to get by constructing the key directly
                    return GetConnectionStringByKey(dbnameLower);
            }

            return SdiDb.GetConnectionString(database);
        }

        private static string GetConnectionStringByKey(string dbname)
        {
            string key = "db:" + dbname;
            
            // Try specific database first
            if (Snsc.Configuration.ConfigurationManager.DBConnectionStrings.ContainsKey(key))
            {
                string connStr = Snsc.Configuration.ConfigurationManager.DBConnectionStrings[key];
                if (!string.IsNullOrEmpty(connStr))
                    return connStr;
            }
            
            // Fall back to db:core
            return SdiDb.GetConnectionString(DatabaseTarget.Core);
        }
    }
}