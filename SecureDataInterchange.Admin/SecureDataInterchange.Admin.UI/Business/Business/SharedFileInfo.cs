using System;
using System.Data;
using Dapper;
using Snsc.FileTransfers.SecureDataInterchange.Business.Data;

namespace Snsc.FileTransfers.SecureDataInterchange.Business
{
	/// <summary>
	/// Read-only information about a single Shared File.
	/// </summary>
	[Serializable]
	public class SharedFileInfo
	{
		#region Business Methods and Properties (6) 

		// Properties (6) 

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Description { get; private set; }

		/// <summary>
		/// Gets the name of the disk file.
		/// </summary>
		/// <value>The name of the disk file.</value>
		public string DiskFileName { get; private set; }

		/// <summary>
		/// Gets the ID.
		/// </summary>
		/// <value>The ID.</value>
		public int ID { get; private set; }

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; private set; }

		/// <summary>
		/// Gets or sets the uploaded by.
		/// </summary>
		/// <value>The uploaded by.</value>
		public string UploadedBy { get; private set; }

        public Guid? UploadedByAspNetUser { get; private set; }
        public Int32? UploadedByAmsUser { get; private set; }

        public decimal? FileSizeBytes { get; private set; }
        public DateTime CreatedOnUTC { get; private set; }

		/// <summary>
		/// Gets the zone ID.
		/// </summary>
		/// <value>The zone ID.</value>
		public int ZoneID { get; private set; }


		#endregion Business Methods and Properties 

		#region Factory Methods / Construction (2) 

		/// <summary>
		/// Gets the shared file.
		/// </summary>
		/// <param name="fileInfo">The file info.</param>
		/// <returns></returns>
		internal static SharedFileInfo GetSharedFile(DataRow fileInfo)
		{
			return new SharedFileInfo(fileInfo);
		}

        internal static SharedFileInfo FromRow(FileSharingZoneInfoList.FileRow info)
        {
            return new SharedFileInfo(info);
        }

		private SharedFileInfo(DataRow info)
		{
			this.ID = (int)info["SharedFileID"];
			this.Description = info["Description"] as string ?? "";
			this.DiskFileName = info["DiskFileName"] as string ?? "";
			this.ZoneID = (int)info["FileSharingZoneID"];
			this.Name = info["Name"] as string ?? "";
			this.UploadedBy = info["UploadedBy"] as string ?? "(unknown)";
            this.CreatedOnUTC = Convert.ToDateTime(info["CreatedOnUTC"]).ToLocalTime();

            if (!(info["FileSizeBytes"] is DBNull))
                this.FileSizeBytes = Convert.ToDecimal(info["FileSizeBytes"]);

            if (!(info["UploadedByAmsUser"] is DBNull))
                this.UploadedByAmsUser = Convert.ToInt32(info["UploadedByAmsUser"]);

            if (!(info["UploadedByAspNetUser"] is DBNull))
                this.UploadedByAspNetUser = new Guid(info["UploadedByAspNetUser"].ToString());

		}

		private SharedFileInfo(FileSharingZoneInfoList.FileRow info)
		{
			this.ID = info.SharedFileID;
			this.Description = info.Description ?? string.Empty;
			this.DiskFileName = info.DiskFileName ?? string.Empty;
			this.ZoneID = info.FileSharingZoneID;
			this.Name = info.Name ?? string.Empty;
			this.UploadedBy = info.UploadedBy ?? "(unknown)";
			this.CreatedOnUTC = info.CreatedOnUTC.ToLocalTime();
			this.FileSizeBytes = info.FileSizeBytes;
			this.UploadedByAmsUser = info.UploadedByAmsUser;
			this.UploadedByAspNetUser = info.UploadedByAspNetUser;
		}

		#endregion Factory Methods / Construction 

        public static void DeleteFileByID(int sharedFileID)
        {
			using (var connection = SdiDb.CreateOpenConnection(DatabaseTarget.Core))
            {
				connection.Execute(
					"SecureShare.up_SDId_sdiSharedFile",
					new { SharedFileID = sharedFileID },
					commandType: CommandType.StoredProcedure);
            }
        }

	}
}
