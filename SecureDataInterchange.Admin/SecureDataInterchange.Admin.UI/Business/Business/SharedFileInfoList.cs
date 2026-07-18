using System;
using System.Collections.Generic;
using System.Data;

namespace Snsc.FileTransfers.SecureDataInterchange.Business
{
	[Serializable]
	public class SharedFileInfoList : List<SharedFileInfo>
	{
		#region�Factory�Methods�/�Construction�(3)�

		/// <summary>
		/// Gets the shared file list.
		/// </summary>
		/// <param name="files">The files.</param>
		/// <returns></returns>
		internal static SharedFileInfoList GetSharedFileList(DataRow[] files)
		{
			return new SharedFileInfoList(files);
		}

	internal static SharedFileInfoList FromRows(IEnumerable<FileSharingZoneInfoList.FileRow> files)
	{
		var list = new SharedFileInfoList();
		if (files != null)
		{
			foreach (var item in files)
			{
				list.Add(SharedFileInfo.FromRow(item));
			}
		}
		return list;
	}

	/// <summary>
	/// Gets files for a specific zone by calling the stored procedure.
	/// This is used for lazy-loading files on-demand.
	/// </summary>
	/// <param name="zoneID">The zone ID to get files for.</param>
	/// <returns>List of files in the zone.</returns>
	internal static SharedFileInfoList GetFilesByZoneID(int zoneID)
	{
		using (var connection = Data.SdiDb.CreateOpenConnection(Data.DatabaseTarget.Core))
		{
			var files = Dapper.SqlMapper.Query<FileSharingZoneInfoList.FileRow>(
				connection,
				"SecureShare.up_SDIs_SharedFilesByZone",
				new 
				{ 
					FileSharingZoneID = zoneID,
					AmsUserID = Snsc.Security.SnscPrincipal.SNIdentity.AmsUserID ?? 0
				},
				commandType: System.Data.CommandType.StoredProcedure);

			return FromRows(files);
		}
	}

		private SharedFileInfoList() { }

		private SharedFileInfoList(DataRow[] files)
		{
			foreach (var item in files)
			{
				this.Add(SharedFileInfo.GetSharedFile(item));
			}
		}

		#endregion�Factory�Methods�/�Construction�
	}
}
