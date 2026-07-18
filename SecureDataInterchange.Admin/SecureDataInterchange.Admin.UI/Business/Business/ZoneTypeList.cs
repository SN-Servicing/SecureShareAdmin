using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Snsc.FileTransfers.SecureDataInterchange.Business.Data;

namespace Snsc.FileTransfers.SecureDataInterchange.Business
{ 
	[Serializable]
	public class ZoneTypeList : List<ZoneType>
	{
		internal static ZoneTypeList GetZoneTypeList(int amsUserID)
        {
			var result = new ZoneTypeList();
			using (var connection = SdiDb.CreateOpenConnection(DatabaseTarget.Core))
			{
				IEnumerable<ZoneType> rows = connection.Query<ZoneType>(
					"SecureShare.up_SDIs_FileSharingAdminZoneTypesForAmsUser",
					new { AmsUserID = amsUserID },
					commandType: CommandType.StoredProcedure);
				result.AddRange(rows);
			}

			return result;
        }
	}
}
