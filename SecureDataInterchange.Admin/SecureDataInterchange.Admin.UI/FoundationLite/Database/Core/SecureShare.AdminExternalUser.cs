using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Snsc.Base.Data;

namespace Snsc.Database.Core
{
    // -------------------------------------------------------------------------
    // SecureShare_CreateExternalUser_Admin
    // TODO: Verify stored procedure name and parameters against the database.
    // -------------------------------------------------------------------------
    [Serializable]
    public class SecureShare_CreateExternalUser_Admin : StoredProcedureExecutorBase
    {
        public void VersionInfo7FF663188C8A3613F27BC1C451C7439C() { VersionInfoChecked__ = true; }

        private void Init()
        {
            this.dbname = "Core";
            this.procname = "[SecureShare].[CreateExternalUser_Admin]";
            InitializeParameters();
        }

        public SecureShare_CreateExternalUser_Admin() { Init(); }
        public SecureShare_CreateExternalUser_Admin(SqlConnection cn) : base(cn) { Init(); }
        public SecureShare_CreateExternalUser_Admin(SqlConnection cn, SqlTransaction tx) : base(cn, tx) { Init(); }
        protected SecureShare_CreateExternalUser_Admin(SerializationInfo info, StreamingContext context) : base(info, context) { }

        private SecureShare_CreateExternalUser_AdminParameters __paramsInstance;
        public SecureShare_CreateExternalUser_AdminParameters Parameters
        {
            get
            {
                if (__paramsInstance == null) throw new ParametersNotInitializedException(this);
                return __paramsInstance;
            }
        }

        protected override void InitializeParameters()
        {
            __paramsInstance = new SecureShare_CreateExternalUser_AdminParameters(Command);
        }

        [Serializable]
        public class SecureShare_CreateExternalUser_AdminParameters
        {
            private SqlParameter _UserName, _FirstName, _LastName, _UserAccess, _ExternalUserId, _ReturnValue;

            public SecureShare_CreateExternalUser_AdminParameters(SqlCommand cmd)
            {
                SqlParameter p;
                p = new SqlParameter(); p.ParameterName = "@UserName"; p.SqlDbType = SqlDbType.VarChar; p.Size = 100; p.Direction = ParameterDirection.Input; cmd.Parameters.Add(p); _UserName = p;
                p = new SqlParameter(); p.ParameterName = "@FirstName"; p.SqlDbType = SqlDbType.VarChar; p.Size = 100; p.Direction = ParameterDirection.Input; cmd.Parameters.Add(p); _FirstName = p;
                p = new SqlParameter(); p.ParameterName = "@LastName"; p.SqlDbType = SqlDbType.VarChar; p.Size = 100; p.Direction = ParameterDirection.Input; cmd.Parameters.Add(p); _LastName = p;
                p = new SqlParameter(); p.ParameterName = "@UserAccess"; p.SqlDbType = SqlDbType.Int; p.Direction = ParameterDirection.Input; cmd.Parameters.Add(p); _UserAccess = p;
                p = new SqlParameter(); p.ParameterName = "@ExternalUserId"; p.SqlDbType = SqlDbType.UniqueIdentifier; p.Direction = ParameterDirection.Output; cmd.Parameters.Add(p); _ExternalUserId = p;
                p = new SqlParameter(); p.ParameterName = "@RETURN_VALUE"; p.SqlDbType = SqlDbType.Int; p.Direction = ParameterDirection.ReturnValue; cmd.Parameters.Add(p); _ReturnValue = p;
            }

            public SqlParameter UserName_ { get { return _UserName; } }
            public SqlParameter FirstName_ { get { return _FirstName; } }
            public SqlParameter LastName_ { get { return _LastName; } }
            public SqlParameter UserAccess_ { get { return _UserAccess; } }
            public SqlParameter ExternalUserId_ { get { return _ExternalUserId; } }
            public SqlParameter ReturnValue__ { get { return _ReturnValue; } }
        }
    }

    // -------------------------------------------------------------------------
    // SecureShare_GetExternalUsersByName_Admin
    // TODO: Verify stored procedure name and parameters against the database.
    // -------------------------------------------------------------------------
    [Serializable]
    public class SecureShare_GetExternalUsersByName_Admin : StoredProcedureExecutorBase
    {
        public void VersionInfoA994FE6E111B56A7688B8EFF3992A4F() { VersionInfoChecked__ = true; }

        private void Init()
        {
            this.dbname = "Core";
            this.procname = "[SecureShare].[GetExternalUsersByName_Admin]";
            InitializeParameters();
        }

        public SecureShare_GetExternalUsersByName_Admin() { Init(); }
        public SecureShare_GetExternalUsersByName_Admin(SqlConnection cn) : base(cn) { Init(); }
        public SecureShare_GetExternalUsersByName_Admin(SqlConnection cn, SqlTransaction tx) : base(cn, tx) { Init(); }
        protected SecureShare_GetExternalUsersByName_Admin(SerializationInfo info, StreamingContext context) : base(info, context) { }

        private SecureShare_GetExternalUsersByName_AdminParameters __paramsInstance;
        public SecureShare_GetExternalUsersByName_AdminParameters Parameters
        {
            get
            {
                if (__paramsInstance == null) throw new ParametersNotInitializedException(this);
                return __paramsInstance;
            }
        }

        protected override void InitializeParameters()
        {
            __paramsInstance = new SecureShare_GetExternalUsersByName_AdminParameters(Command);
        }

        [Serializable]
        public class SecureShare_GetExternalUsersByName_AdminParameters
        {
            private SqlParameter _PageSize, _PageIndex, _UserNameToMatch, _ReturnValue;

            public SecureShare_GetExternalUsersByName_AdminParameters(SqlCommand cmd)
            {
                SqlParameter p;
                p = new SqlParameter(); p.ParameterName = "@PageSize"; p.SqlDbType = SqlDbType.Int; p.Direction = ParameterDirection.Input; cmd.Parameters.Add(p); _PageSize = p;
                p = new SqlParameter(); p.ParameterName = "@PageIndex"; p.SqlDbType = SqlDbType.Int; p.Direction = ParameterDirection.Input; cmd.Parameters.Add(p); _PageIndex = p;
                p = new SqlParameter(); p.ParameterName = "@UserNameToMatch"; p.SqlDbType = SqlDbType.VarChar; p.Size = 256; p.Direction = ParameterDirection.Input; cmd.Parameters.Add(p); _UserNameToMatch = p;
                p = new SqlParameter(); p.ParameterName = "@RETURN_VALUE"; p.SqlDbType = SqlDbType.Int; p.Direction = ParameterDirection.ReturnValue; cmd.Parameters.Add(p); _ReturnValue = p;
            }

            public SqlParameter PageSize_ { get { return _PageSize; } }
            public SqlParameter PageIndex_ { get { return _PageIndex; } }
            public SqlParameter UserNameToMatch_ { get { return _UserNameToMatch; } }
            public SqlParameter ReturnValue__ { get { return _ReturnValue; } }
        }
    }

    // -------------------------------------------------------------------------
    // SecureShare_UpdateExternalUser_Admin
    // TODO: Verify stored procedure name and parameters against the database.
    // -------------------------------------------------------------------------
    [Serializable]
    public class SecureShare_UpdateExternalUser_Admin : StoredProcedureExecutorBase
    {
        public void VersionInfoE68FA35B08F17139AF753F57ADFA2A2() { VersionInfoChecked__ = true; }

        private void Init()
        {
            this.dbname = "Core";
            this.procname = "[SecureShare].[UpdateExternalUser_Admin]";
            InitializeParameters();
        }

        public SecureShare_UpdateExternalUser_Admin() { Init(); }
        public SecureShare_UpdateExternalUser_Admin(SqlConnection cn) : base(cn) { Init(); }
        public SecureShare_UpdateExternalUser_Admin(SqlConnection cn, SqlTransaction tx) : base(cn, tx) { Init(); }
        protected SecureShare_UpdateExternalUser_Admin(SerializationInfo info, StreamingContext context) : base(info, context) { }

        private SecureShare_UpdateExternalUser_AdminParameters __paramsInstance;
        public SecureShare_UpdateExternalUser_AdminParameters Parameters
        {
            get
            {
                if (__paramsInstance == null) throw new ParametersNotInitializedException(this);
                return __paramsInstance;
            }
        }

        protected override void InitializeParameters()
        {
            __paramsInstance = new SecureShare_UpdateExternalUser_AdminParameters(Command);
        }

        [Serializable]
        public class SecureShare_UpdateExternalUser_AdminParameters
        {
            private SqlParameter _UserName, _FirstName, _LastName, _UserAccess, _ExternalUserDisabledReasonId;

            public SecureShare_UpdateExternalUser_AdminParameters(SqlCommand cmd)
            {
                SqlParameter p;
                p = new SqlParameter(); p.ParameterName = "@UserName"; p.SqlDbType = SqlDbType.VarChar; p.Size = 100; p.Direction = ParameterDirection.Input; cmd.Parameters.Add(p); _UserName = p;
                p = new SqlParameter(); p.ParameterName = "@FirstName"; p.SqlDbType = SqlDbType.VarChar; p.Size = 100; p.Direction = ParameterDirection.Input; cmd.Parameters.Add(p); _FirstName = p;
                p = new SqlParameter(); p.ParameterName = "@LastName"; p.SqlDbType = SqlDbType.VarChar; p.Size = 100; p.Direction = ParameterDirection.Input; cmd.Parameters.Add(p); _LastName = p;
                p = new SqlParameter(); p.ParameterName = "@UserAccess"; p.SqlDbType = SqlDbType.Int; p.Direction = ParameterDirection.Input; cmd.Parameters.Add(p); _UserAccess = p;
                p = new SqlParameter(); p.ParameterName = "@ExternalUserDisabledReasonId"; p.SqlDbType = SqlDbType.Int; p.Direction = ParameterDirection.Input; cmd.Parameters.Add(p); _ExternalUserDisabledReasonId = p;
            }

            public SqlParameter UserName_ { get { return _UserName; } }
            public SqlParameter FirstName_ { get { return _FirstName; } }
            public SqlParameter LastName_ { get { return _LastName; } }
            public SqlParameter UserAccess_ { get { return _UserAccess; } }
            public SqlParameter ExternalUserDisabledReasonId_ { get { return _ExternalUserDisabledReasonId; } }
        }
    }
}
