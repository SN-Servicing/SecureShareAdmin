using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Snsc.FileTransfers.SecureDataInterchange.Business.Data;

namespace Snsc.FileTransfers.SecureDataInterchange.Business
{
	[Serializable]
    public class AdministrationData
	{
		public static AdministrationData GetAdministrationData(int amsUserID)
		{
            return new AdministrationData
            {
                UserZoneInfo = UserZoneInfo.GetZoneInfoForInternalUser(amsUserID),
                ZoneTypes = ZoneTypeList.GetZoneTypeList(amsUserID)
            };
		}
        private AdministrationData() { }

        public UserZoneInfo UserZoneInfo
        {
            get;
            private set;
        }

        public ZoneTypeList ZoneTypes
        {
            get;
            private set;
        }

        public AspNetUserList GetAspNetUsersByZoneID(int zoneID)
        {
            return AspNetUserList.GetAspNetUsersByZoneID(zoneID);
        }

        public static void InsertFileSharingZonePermission(int fileSharingZoneID, Guid userID, string userName)
        {
            FileSharingZonePermission.InsertPermission(fileSharingZoneID, userID, userName);
        }

        public static int? InsertFileSharingZone(string primaryObjectID, string secondaryObjectID, int fileSharingZoneTypeID, string description  )
        {
            int? fileSharingZoneID = -1;
            fileSharingZoneID = FileSharingZone.InsertFileSharingZone(primaryObjectID, secondaryObjectID, fileSharingZoneTypeID, description);

            return fileSharingZoneID;
        }

        public static int UpdateFileSharingZoneInfo(int zoneID, int zoneTypeID, string description, string primaryValue, string secondaryValue)
        {
            return UpdateFileSharingZone.UpdateZone(zoneID, zoneTypeID, description, primaryValue, secondaryValue);
        }

        public static void DeleteFileSharingZoneByID(int zoneID)
        {
            DeleteFileSharingZone.DeleteZone(zoneID);
        }

        [Serializable]
        public class FileSharingZonePermission
        {
            public int FileSharingZonePermissionID { get; private set; }
            public int FileSharingZoneID { get; private set; }
            public Guid UserID { get; private set; }

            public static void InsertPermission(int fileSharingZoneID, Guid userID, string userName)
            {
                FileSharingZonePermission p = new FileSharingZonePermission();
                p.FileSharingZoneID = fileSharingZoneID;
                p.UserID = userID;

                p.Execute();

                Foundation.SDI.Logging.InsertNewLog(Foundation.Enumerations.SdiLogSubType.ZoneAccessAdded,
                   userName, fileSharingZoneID.ToString(), Snsc.Security.SnscPrincipal.SNIdentity.AmsUserID, 
                   null, Snsc.Foundation.Enumerations.SdiLogSource.SdiInternalAdmin);
            }

            private void Execute()
            {
                using (var connection = SdiDb.CreateOpenConnection(DatabaseTarget.Core))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ZonePermissionID", null, DbType.Int32, ParameterDirection.InputOutput, null);
                    parameters.Add("@FileSharingZoneID", this.FileSharingZoneID, DbType.Int32, ParameterDirection.Input, null);
                    parameters.Add("@AspNetUserID", this.UserID, DbType.Guid, ParameterDirection.Input, null);

                    connection.Execute(
                        "SecureShare.up_SDIi_sdiFileSharingZonePermission",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    FileSharingZonePermissionID = parameters.Get<int>("@ZonePermissionID");
                }
            }

        }

        [Serializable]
        public class FileSharingZone
        {
            public string PrimaryObjectID { get; private set; }
            public string SecondaryObjectID { get; private set; }
            public string Description { get; private set; }
            public int FileSharingZoneTypeID { get; private set; }
            public int FileSharingZoneID { get; private set; }

            public static int InsertFileSharingZone(string primaryObjectID, string secondaryObjectID, int fileSharingZoneTypeID, string description)
            {
                FileSharingZone fsz = new FileSharingZone();
                fsz.Description = description;
                fsz.FileSharingZoneTypeID = fileSharingZoneTypeID;
                fsz.PrimaryObjectID = primaryObjectID;
                fsz.SecondaryObjectID = secondaryObjectID;

                try
                {
                    fsz.Execute();
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627)
                    {
                        return -1; //indicates a violation of the unique constraint
                    }

                    throw;
                }

                Foundation.SDI.Logging.InsertNewLog(Foundation.Enumerations.SdiLogSubType.ZoneAdded,
                   null, null, Snsc.Security.SnscPrincipal.SNIdentity.AmsUserID, 
                   null, Snsc.Foundation.Enumerations.SdiLogSource.SdiInternalAdmin);

                return fsz.FileSharingZoneID;
            }

            private void Execute()
            {
                using (var connection = SdiDb.CreateOpenConnection(DatabaseTarget.Core))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@FileSharingZoneID", null, DbType.Int32, ParameterDirection.InputOutput, null);
                    parameters.Add("@PrimaryObjectID", this.PrimaryObjectID, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add("@SecondaryObjectID", this.SecondaryObjectID, DbType.String, ParameterDirection.Input, 100);
                    parameters.Add("@FileSharingZoneTypeID", this.FileSharingZoneTypeID, DbType.Int32, ParameterDirection.Input, null);
                    parameters.Add("@Description", this.Description, DbType.String, ParameterDirection.Input, 1024);

                    connection.Execute(
                        "SecureShare.up_SDIi_sdiFileSharingZone",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    this.FileSharingZoneID = parameters.Get<int>("@FileSharingZoneID");
                }
            }
        }

        [Serializable]
        public class DeleteZonePermission
        {
            public int ZonePermissionID { get; private set; }

            public static void DeleteZonePermissionByID(int zonePermissionID, string userBeingDeleted, int zoneID)
            {
                DeleteZonePermission dz = new DeleteZonePermission();
                dz.ZonePermissionID = zonePermissionID;
                dz.Execute();

                Foundation.SDI.Logging.InsertNewLog(Foundation.Enumerations.SdiLogSubType.ZoneAccessRemoved, 
                    userBeingDeleted, zoneID.ToString(), Snsc.Security.SnscPrincipal.SNIdentity.AmsUserID,
                    null, Snsc.Foundation.Enumerations.SdiLogSource.SdiInternalAdmin);
            }

            private void Execute()
            {
                using (var connection = SdiDb.CreateOpenConnection(DatabaseTarget.Core))
                {
                    connection.Execute(
                        "SecureShare.up_SDId_sdiFileSharingZonePermission",
                        new { ZonePermissionID = this.ZonePermissionID },
                        commandType: CommandType.StoredProcedure);
                }
            }
        }

        [Serializable]
        private class UpdateFileSharingZone
        {
            public int ZoneID { get; private set; }
            public int ZoneTypeID { get; private set; }
            public string Description { get; private set; }
            public string PrimaryObjectValue { get; private set; }
            public string SecondaryObjectValue { get; private set; }

            public static int UpdateZone(int zoneID, int zoneTypeID, string description, string primaryValue, string secondaryValue)
            {
                UpdateFileSharingZone u = new UpdateFileSharingZone();
                u.ZoneID = zoneID;
                u.ZoneTypeID = zoneTypeID;
                u.PrimaryObjectValue = primaryValue;
                u.SecondaryObjectValue = secondaryValue;
                u.Description = description;

                try
                {
                    u.Execute();
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627)
                    {
                        return -1;
                    }

                    throw;
                }

                return 0; //ok
            }

            private void Execute()
            {
                using (var connection = SdiDb.CreateOpenConnection(DatabaseTarget.Core))
                {
                    connection.Execute(
                        "SecureShare.up_SDIu_sdiFileSharingZone",
                        new
                        {
                            FileSharingZoneID = ZoneID,
                            FileSharingZoneTypeID = ZoneTypeID,
                            Description,
                            PrimaryObjectID = PrimaryObjectValue,
                            SecondaryObjectID = SecondaryObjectValue
                        },
                        commandType: CommandType.StoredProcedure);
                }
            }

        }

        [Serializable]
        private class DeleteFileSharingZone
        {
            public int ZoneID { get; private set; }

            public static void DeleteZone(int zoneID)
            {
                DeleteFileSharingZone d = new DeleteFileSharingZone();
                d.ZoneID = zoneID;

                d.Execute();

                Foundation.SDI.Logging.InsertNewLog(Foundation.Enumerations.SdiLogSubType.ZoneDeleted,
                   zoneID.ToString(), null, Snsc.Security.SnscPrincipal.SNIdentity.AmsUserID, 
                   null, Snsc.Foundation.Enumerations.SdiLogSource.SdiInternalAdmin);
            }

            private void Execute()
            {
                using (var connection = SdiDb.CreateOpenConnection(DatabaseTarget.Core))
                {
                    connection.Execute(
                        "SecureShare.up_SDId_sdiFileSharingZone",
                        new { FileSharingZoneID = ZoneID },
                        commandType: CommandType.StoredProcedure);
                }
            }
        }

	}
}
