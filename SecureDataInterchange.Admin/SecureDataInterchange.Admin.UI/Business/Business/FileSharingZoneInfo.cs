using Dapper;
using Snsc.FileTransfers.SecureDataInterchange.Business.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace Snsc.FileTransfers.SecureDataInterchange.Business
{
    public class FileSharingZoneInfo
    {
        private SharedFileInfoList _files;

        private FileSharingZoneInfo(DataRow zoneData, DataRow[] files)
        {
            this.ZoneID = Convert.ToInt32(zoneData["FileSharingZoneID"]);
            this.FileCount = Convert.ToInt32(zoneData["FileCount"]);
            this.ZoneTypeName = Convert.ToString(zoneData["ZoneTypeName"]);
            this.Description = Convert.ToString(zoneData["Description"]);
            this.PrimaryFieldName = Convert.ToString(zoneData["PrimaryObjectIDFieldName"]);
            this.PrimaryIDValue = Convert.ToString(zoneData["PrimaryObjectID"]);
            this.SecondaryFieldName = Convert.ToString(zoneData["SecondaryObjectIDFieldName"]);
            this.SecondaryIDValue = Convert.ToString(zoneData["SecondaryObjectID"]);
            this.ZonePermissionID = Convert.ToInt32((zoneData["ZonePermissionID"] is System.DBNull) ? -1 : zoneData["ZonePermissionID"]);
            this.ZoneTypeID = Convert.ToInt32((zoneData["FileSharingZoneTypeID"] is System.DBNull) ? -1 : zoneData["FileSharingZoneTypeID"]);

            if (zoneData["NotificationOptIn"] != null)
                this.NotificationOptIn = Convert.ToBoolean(zoneData["NotificationOptIn"]);

            if (zoneData["LastUploadDateUTC"] is DateTime)
            {
                this.LastUploadDate = ((DateTime)zoneData["LastUploadDateUTC"]).ToLocalTime();
            }
            else
            {
                this.LastUploadDate = DateTime.MinValue;
            }

            // Pre-populate files if provided (legacy DataRow[] constructor)
            if (files != null && files.Length > 0)
            {
                this._files = SharedFileInfoList.GetSharedFileList(files);
            }
        }

        private FileSharingZoneInfo(FileSharingZoneInfoList.ZoneRow zoneData, IEnumerable<FileSharingZoneInfoList.FileRow> files)
        {
            this.ZoneID = zoneData.FileSharingZoneID;
            this.FileCount = zoneData.FileCount;
            this.ZoneTypeName = zoneData.ZoneTypeName ?? string.Empty;
            this.Description = zoneData.Description ?? string.Empty;
            this.PrimaryFieldName = zoneData.PrimaryObjectIDFieldName ?? string.Empty;
            this.PrimaryIDValue = zoneData.PrimaryObjectID ?? string.Empty;
            this.SecondaryFieldName = zoneData.SecondaryObjectIDFieldName ?? string.Empty;
            this.SecondaryIDValue = zoneData.SecondaryObjectID ?? string.Empty;
            this.ZoneTypeID = zoneData.FileSharingZoneTypeID ?? -1;
            this.NotificationOptIn = zoneData.NotificationOptIn ?? false;

            // ZonePermissionID and LastUploadDate not provided by zones-only stored procs
            this.ZonePermissionID = -1;
            this.LastUploadDate = DateTime.MinValue;

            // Pre-populate files if provided, otherwise they'll be lazy-loaded
            if (files != null)
            {
                this._files = SharedFileInfoList.FromRows(files);
            }
        }

        public string Description { get; private set; }

        public int FileCount { get; private set; }

        /// <summary>
        /// Gets the files for this zone. Files are lazy-loaded on first access.
        /// </summary>
        public SharedFileInfoList Files
        {
            get
            {
                if (_files == null)
                {
                    _files = SharedFileInfoList.GetFilesByZoneID(this.ZoneID);
                }
                return _files;
            }
            private set { _files = value; }
        }

        public DateTime LastUploadDate { get; private set; }

        public bool NotificationOptIn { get; private set; }
        public string PrimaryFieldName { get; private set; }

        public string PrimaryIDValue { get; private set; }

        public string SecondaryFieldName { get; private set; }

        public string SecondaryIDValue { get; private set; }

        public int ZoneID { get; private set; }

        public int ZonePermissionID { get; private set; }
        public int ZoneTypeID { get; private set; }
        public string ZoneTypeName { get; private set; }

        public static void ChangeEmailNotification(int fileSharingZonePermissionID, bool notificationOptIn, Guid? aspNetUserID, int? amsUserID)
        {
            FileSharingZonePermissionExecutor.UpdateEmailNotification(fileSharingZonePermissionID,
                notificationOptIn, aspNetUserID, amsUserID);
        }

        public static void ChangeEmailNotificationAmsUser(int fileSharingZone, bool notificationOptIn, int? amsUserID)
        {
            FileSharingZonePermissionAmsUserExecutor.UpdateEmailNotification(fileSharingZone, notificationOptIn, amsUserID);
        }

        public static FileSharingZoneInfo GetFileSharingZoneInfo(DataRow zoneRow, DataRow[] files)
        {
            return new FileSharingZoneInfo(zoneRow, files);
        }

        internal static FileSharingZoneInfo FromRows(FileSharingZoneInfoList.ZoneRow zoneData, IEnumerable<FileSharingZoneInfoList.FileRow> files)
        {
            return new FileSharingZoneInfo(zoneData, files);
        }

        public class FileSharingZonePermissionAmsUserExecutor
        {
            private FileSharingZonePermissionAmsUserExecutor(int fileSharingZoneID, bool notificationOptIn, int? amsUserID)
            {
                this.NotificationOptIn = notificationOptIn;
                this.FileSharingZoneID = fileSharingZoneID;
                this.AmsUserID = amsUserID;
            }

            public int? AmsUserID { get; private set; }
            public int FileSharingZoneID { get; private set; }
            public bool NotificationOptIn { get; private set; }

            public static void UpdateEmailNotification(int fileSharingZoneID, bool notificationOptIn, int? amsUserID)
            {
                FileSharingZonePermissionAmsUserExecutor fszpe = new FileSharingZonePermissionAmsUserExecutor(
                    fileSharingZoneID, notificationOptIn, amsUserID);
                fszpe.Execute();
            }

            private void Execute()
            {
                using (var connection = SdiDb.CreateOpenConnection(DatabaseTarget.Core))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@FileSharingZoneID", this.FileSharingZoneID, DbType.Int32, ParameterDirection.Input, null);
                    parameters.Add("@AmsUserID", this.AmsUserID, DbType.Int64, ParameterDirection.Input, null);
                    parameters.Add("@NotificationOptIn", this.NotificationOptIn, DbType.Boolean, ParameterDirection.Input, null);
                    parameters.Add("@nullMap", null, DbType.Binary, ParameterDirection.Input, 1);

                    connection.Execute(
                        "SecureShare.up_SDIiu_sdiFileSharingZonePermissionAmsUser",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    Foundation.SDI.Logging.InsertNewLog(
                        (this.NotificationOptIn) ? Foundation.Enumerations.SdiLogSubType.NotificationOptIn : Foundation.Enumerations.SdiLogSubType.NotificationOptOut,
                        FileSharingZoneID.ToString(), "Internal User",
                        AmsUserID, null, Snsc.Foundation.Enumerations.SdiLogSource.SdiInternalAdmin);
                }
            }
        }

        [Serializable]
        public class FileSharingZonePermissionExecutor
        {
            private FileSharingZonePermissionExecutor(int zonePermissionID, bool notificationOptIn, Guid? aspNetUserID, int? amsUserID)
            {
                this.NotificationOptIn = notificationOptIn;
                this.ZonePermissionID = zonePermissionID;
                this.AspNetUserID = aspNetUserID;
                this.AmsUserID = amsUserID;
            }

            public int? AmsUserID { get; private set; }
            public Guid? AspNetUserID { get; private set; }
            public bool NotificationOptIn { get; private set; }
            public int ZonePermissionID { get; private set; }

            public static void UpdateEmailNotification(int zonePermissionID, bool notificationOptIn, Guid? aspNetUserID, int? amsUserID)
            {
                FileSharingZonePermissionExecutor fszpe = new FileSharingZonePermissionExecutor(
                    zonePermissionID, notificationOptIn, aspNetUserID, amsUserID);
                fszpe.Execute();
            }

            private void Execute()
            {
                using (var connection = SdiDb.CreateOpenConnection(DatabaseTarget.Core))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ZonePermissionID", this.ZonePermissionID, DbType.Int32, ParameterDirection.Input, null);
                    parameters.Add("@FileSharingZoneID", null, DbType.Int32, ParameterDirection.Input, null);
                    parameters.Add("@AspNetUserID", this.AspNetUserID, DbType.Guid, ParameterDirection.Input, null);
                    parameters.Add("@NotificationOptIn", this.NotificationOptIn, DbType.Boolean, ParameterDirection.Input, null);
                    parameters.Add("@nullMap", null, DbType.Binary, ParameterDirection.Input, 1);

                    connection.Execute(
                        "SecureShare.up_SDIu_sdiFileSharingZonePermission",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    Foundation.SDI.Logging.InsertNewLog(
                        (this.NotificationOptIn) ? Foundation.Enumerations.SdiLogSubType.NotificationOptIn : Foundation.Enumerations.SdiLogSubType.NotificationOptOut,
                        ZonePermissionID.ToString(), null,
                        AmsUserID, AspNetUserID,
                        AmsUserID.HasValue ? Foundation.Enumerations.SdiLogSource.SdiInternalAdmin : Snsc.Foundation.Enumerations.SdiLogSource.SdiWebSite);
                }
            }
        }
    }
}