using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Snsc.FileTransfers.SecureDataInterchange.Business.Data;

namespace Snsc.FileTransfers.SecureDataInterchange.Business
{
    [Serializable]
    public class FileSharingZoneInfoList : List<FileSharingZoneInfo>
    {
        /// <summary>
        /// Gets zones for an external user. Files are lazy-loaded on-demand.
        /// </summary>
        internal static FileSharingZoneInfoList GetFileSharingZoneInfoList(Guid userID)
        {
            return GetZonesOnly(
                "SecureShare.up_SDIs_FileSharingZonesForUser",
                new { AspNetUserID = userID });
        }

        /// <summary>
        /// Gets zones for an internal admin user. Files are lazy-loaded on-demand.
        /// </summary>
        internal static FileSharingZoneInfoList GetFileSharingZoneInfoListForInternalUser(int amsUserID)
        {
            return GetZonesOnly(
                "SecureShare.up_SDIs_FileSharingAdminZonesForAmsUser",
                new { AmsUserID = amsUserID });
        }

        /// <summary>
        /// Gets zones WITHOUT files using a stored procedure that returns only zone data.
        /// Files will be lazy-loaded on-demand when accessed via the Files property.
        /// </summary>
        private static FileSharingZoneInfoList GetZonesOnly(string storedProcedure, object parameters)
        {
            var list = new FileSharingZoneInfoList();
            using (var connection = SdiDb.CreateOpenConnection(DatabaseTarget.Core))
            {
                var zones = connection.Query<ZoneRow>(
                    storedProcedure,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                foreach (ZoneRow zone in zones)
                {
                    // Don't pass files - they'll be lazy-loaded when Files property is accessed
                    list.Add(FileSharingZoneInfo.FromRows(zone, null));
                }
            }

            return list;
        }

        internal class FileRow
        {
            public DateTime CreatedOnUTC { get; set; }
            public string Description { get; set; }
            public string DiskFileName { get; set; }
            public int FileSharingZoneID { get; set; }
            public decimal? FileSizeBytes { get; set; }
            public string Name { get; set; }
            public int SharedFileID { get; set; }
            public string UploadedBy { get; set; }
            public int? UploadedByAmsUser { get; set; }
            public Guid? UploadedByAspNetUser { get; set; }
        }

        internal class ZoneRow
        {
            public string Description { get; set; }
            public int FileCount { get; set; }
            public int FileSharingZoneID { get; set; }

            //public int? ZonePermissionID { get; set; }
            public int? FileSharingZoneTypeID { get; set; }

            public bool? NotificationOptIn { get; set; }
            public string PrimaryObjectID { get; set; }
            public string PrimaryObjectIDFieldName { get; set; }
            public string SecondaryObjectID { get; set; }
            public string SecondaryObjectIDFieldName { get; set; }
            public string ZoneTypeName { get; set; }
            //public DateTime? LastUploadDateUTC { get; set; }
        }
    }
}