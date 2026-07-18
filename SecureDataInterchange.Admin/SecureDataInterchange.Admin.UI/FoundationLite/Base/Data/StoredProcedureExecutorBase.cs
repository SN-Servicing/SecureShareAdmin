/********************************************
Copyright  2015, SN Servicing, Inc.
********************************************/

using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Snsc.Base.Data
{
    /// <summary>
    /// Base class for stored procedure executor wrappers.
    /// Provides core functionality for executing SQL Server stored procedures.
    /// </summary>
    [Serializable]
    public abstract class StoredProcedureExecutorBase : ISerializable
    {
        protected string dbname;
        protected string md5VersionInfo;
        protected string procname;
        protected int versionInfo__;
        protected bool VersionInfoChecked__;

        private SqlCommand _command;
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected StoredProcedureExecutorBase()
        {
        }

        /// <summary>
        /// Constructor with connection.
        /// </summary>
        /// <param name="cn">SqlConnection to use when executing the procedure.</param>
        protected StoredProcedureExecutorBase(SqlConnection cn)
        {
            _connection = cn;
        }

        /// <summary>
        /// Constructor with connection and transaction.
        /// </summary>
        /// <param name="cn">SqlConnection to use when executing the procedure.</param>
        /// <param name="tx">SqlTransaction to use when executing the procedure.</param>
        protected StoredProcedureExecutorBase(SqlConnection cn, SqlTransaction tx)
        {
            _connection = cn;
            _transaction = tx;
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        protected StoredProcedureExecutorBase(SerializationInfo info, StreamingContext context)
        {
            dbname = info.GetString("dbname");
            procname = info.GetString("procname");
            versionInfo__ = info.GetInt32("versionInfo__");
            md5VersionInfo = info.GetString("md5VersionInfo");
            VersionInfoChecked__ = info.GetBoolean("VersionInfoChecked__");
        }

        /// <summary>
        /// Gets the underlying SqlCommand object.
        /// </summary>
        public SqlCommand Command
        {
            get
            {
                if (_command == null)
                {
                    _command = new SqlCommand();
                    _command.CommandType = CommandType.StoredProcedure;
                }
                return _command;
            }
        }

        /// <summary>
        /// Gets the database name this procedure belongs to.
        /// </summary>
        public string DatabaseName
        { get { return dbname; } }

        /// <summary>
        /// Executes the stored procedure and returns the number of rows affected.
        /// </summary>
        /// <returns>Number of rows affected.</returns>
        public virtual int ExecuteNonQuery()
        {
            ValidateVersionInfo();
            PrepareCommand();
            return Command.ExecuteNonQuery();
        }

        /// <summary>
        /// Executes the stored procedure and returns a SqlDataReader.
        /// </summary>
        /// <returns>SqlDataReader containing results.</returns>
        public virtual SqlDataReader ExecuteReader()
        {
            ValidateVersionInfo();
            PrepareCommand();
            return Command.ExecuteReader();
        }

        /// <summary>
        /// Executes the stored procedure and returns a SqlDataReader with specified behavior.
        /// </summary>
        /// <param name="behavior">CommandBehavior to use.</param>
        /// <returns>SqlDataReader containing results.</returns>
        public virtual SqlDataReader ExecuteReader(CommandBehavior behavior)
        {
            ValidateVersionInfo();
            PrepareCommand();
            return Command.ExecuteReader(behavior);
        }

        /// <summary>
        /// Executes the stored procedure and returns the first column of the first row.
        /// </summary>
        /// <returns>First column of the first row.</returns>
        public virtual object ExecuteScalar()
        {
            ValidateVersionInfo();
            PrepareCommand();
            return Command.ExecuteScalar();
        }

        /// <summary>
        /// Executes the stored procedure and fills a DataSet.
        /// </summary>
        /// <param name="ds">DataSet to fill.</param>
        public virtual void Fill(DataSet ds)
        {
            ValidateVersionInfo();
            PrepareCommand();
            using (SqlDataAdapter adapter = new SqlDataAdapter(Command))
            {
                adapter.Fill(ds);
            }
        }

        /// <summary>
        /// Executes the stored procedure and fills a DataTable.
        /// </summary>
        /// <param name="dt">DataTable to fill.</param>
        public virtual void Fill(DataTable dt)
        {
            ValidateVersionInfo();
            PrepareCommand();
            using (SqlDataAdapter adapter = new SqlDataAdapter(Command))
            {
                adapter.Fill(dt);
            }
        }

        /// <summary>
        /// Serialization method.
        /// </summary>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("dbname", dbname);
            info.AddValue("procname", procname);
            info.AddValue("versionInfo__", versionInfo__);
            info.AddValue("md5VersionInfo", md5VersionInfo);
            info.AddValue("VersionInfoChecked__", VersionInfoChecked__);
        }

        /// <summary>
        /// Sets the connection to use for execution (used by CslaExecutor).
        /// </summary>
        public void SetConnection(SqlConnection cn)
        {
            _connection = cn;
        }

        /// <summary>
        /// Sets up parameters and adds them to the Command object.
        /// Must be implemented by derived classes.
        /// </summary>
        protected abstract void InitializeParameters();

        /// <summary>
        /// Prepares the command for execution by setting connection, transaction, and procedure name.
        /// </summary>
        protected virtual void PrepareCommand()
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("SqlConnection has not been set. Use a constructor that accepts a SqlConnection or set it manually.");
            }

            Command.Connection = _connection;
            Command.Transaction = _transaction;
            Command.CommandText = procname;
            Command.CommandType = CommandType.StoredProcedure;
        }

        /// <summary>
        /// Validates that version info has been checked before execution.
        /// </summary>
        protected virtual void ValidateVersionInfo()
        {
            if (!VersionInfoChecked__)
            {
                throw new VersionInfoNotCheckedException(this);
            }
        }
    }
}